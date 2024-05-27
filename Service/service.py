import numpy as np
import io
import json
import logging
import uvicorn
from fastapi import FastAPI, File, UploadFile, status
from fastapi.encoders import jsonable_encoder
from fastapi.responses import JSONResponse
from PIL import Image
from datacontract.service_config import ServiceConfig
from datacontract.service_output import *
import norfair
from ultralytics import YOLO
import torch
import numpy as np
from ultralytics import YOLO
from ultralytics.utils.plotting import Annotator, colors
import torch
from torchvision import transforms
import os

def classify(image):
        tensor_image = transform(image).unsqueeze(0)
        with torch.no_grad():
            output = classifier(tensor_image)
        _, predicted_idx = torch.max(output, 1)
        class_index = predicted_idx.item()
        return class_names[class_index]


# Настройка логгера
logging.basicConfig()
logger = logging.getLogger(__name__)
logger.setLevel(level=logging.INFO)
app = FastAPI()
service_config_path = r"configs/service_config.json"
print(os.getcwd())
with open(service_config_path, "r") as service_config:
    service_config_json = json.load(service_config)
service_config_adapter = pydantic.TypeAdapter(ServiceConfig)
service_config_python = service_config_adapter.validate_python(service_config_json)
class_names = {0: "Additional signs", 1: "Car", 2:"Forbidding signs" , 3:"Information signs" , 4:"Priority signs", 5:"Warning signs", 6:"Zebra crossing"}
classifier = torch.load(r'resnet152_best_loss.pth', map_location=torch.device('cpu'))
transform = transforms.Compose([
            transforms.Resize(256),
            transforms.CenterCrop(224),
            transforms.ToTensor(),
            transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])
        ])
logger.info(f"Загружен классификатор с путем: {service_config_python.path_to_classifier}")
detector =YOLO(r"best.pt")
logger.info(f"Загружен детектор с путем: {service_config_python.path_to_detector}")

# Определение эндпоинта для проверки состояния сервиса
@app.get(
    "/health",
    tags=["healthcheck"],
    summary="Проверка состояния сервиса",
    response_description="Возвращает HTTP статус 200 (OK)",
    status_code=status.HTTP_200_OK,
)
def health_check() -> str:
    return '{"Status" : "OK"}'


@app.post("/file/")
async def inference(image: UploadFile = File(...)) -> JSONResponse:
    image_content = await image.read()
    pil_image = Image.open(io.BytesIO(image_content))
    if pil_image.mode != 'RGB':
        pil_image = pil_image.convert('RGB')
    cv_image = np.array(pil_image)
    logger.info(f"Принята картинка размерности: {cv_image.shape}")
    output_dict = {"objects": []}
    results = detector.track(cv_image, persist=True, verbose=False)
    boxes = results[0].boxes.xyxy.cpu()
    if results[0].boxes.id is not None:
        clss = results[0].boxes.cls.cpu().tolist()
        track_ids = results[0].boxes.id.int().cpu().tolist()


        annotator = Annotator(cv_image, line_width=2)

        for box, cls, track_id in zip(boxes, clss, track_ids):
            logger.info(f" Принято {box}")
            annotator.box_label(box, color=colors(int(cls), True), label=str(track_id))
            crop_object = cv_image[int(box[1]):int(box[3]), int(box[0]):int(box[2])]
            class_name = classify(Image.fromarray(crop_object))
            output_dict["objects"].append(DetectedObject(xtl=int(box[0]), ytl=int(box[1]), xbr=int(box[2]), ybr=int(box[3]), class_name=class_name, tracked_id=track_id))
            print(cls)
    service_output = ServiceOutput(objects=output_dict["objects"])
    service_output_json = service_output.model_dump(mode="json")
    with open("output_json.json", "w") as output_file:
        json.dump(service_output_json, output_file, indent=4)
    return JSONResponse(content=jsonable_encoder(service_output_json))

uvicorn.run(app, host="localhost", port=7000)
