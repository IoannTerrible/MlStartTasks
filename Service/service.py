import numpy as np
import io
import json
import logging
import uvicorn
from fastapi import FastAPI, File, UploadFile, status
from fastapi.encoders import jsonable_encoder
from fastapi.responses import JSONResponse
from PIL import Image
from classifier import Classifier
from detector import Detector
from datacontract.service_config import ServiceConfig
from datacontract.service_output import *
import norfair
from ultralytics import YOLO
import torch
import cv2
import numpy as np
from ultralytics import YOLO
from ultralytics.utils.plotting import Annotator, colors
from collections import defaultdict
import torch
from torchvision import transforms

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
service_config_path = "configs\\service_config.json"
with open(service_config_path, "r") as service_config:
    service_config_json = json.load(service_config)
service_config_adapter = pydantic.TypeAdapter(ServiceConfig)
service_config_python = service_config_adapter.validate_python(service_config_json)
class_names = {0: "fall", 1: "stand"}
classifier = torch.load('resnet152_best_loss.pth', map_location=torch.device('cpu'))
transform = transforms.Compose([
            transforms.Resize(256),
            transforms.CenterCrop(224),
            transforms.ToTensor(),
            transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])
        ])
logger.info(f"Загружен классификатор с путем: {service_config_python.path_to_classifier}")
detector =YOLO(r"best.pt")
logger.info(f"Загружен детектор с путем: {service_config_python.path_to_detector}")
tracker = norfair.Tracker(distance_function=norfair.distances.iou, distance_threshold=0.7)

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
    detections = []
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
    '''for res in detector_outputs:
        boxes = res.boxes
        for box in boxes:
            # Получение координат ограничивающего прямоугольника
            cords = box.xyxy
            xtl, ytl, xbr, ybr = int(cords[0][0]), int(cords[0][1]), int(cords[0][2]), int(cords[0][3])

            # Вырезание области изображения, соответствующей ограничивающему прямоугольнику
            crop_object = cv_image[ytl:ybr, xtl:xbr]

            # Классификация области изображения
            class_name = classifier.classify(Image.fromarray(crop_object))

            # Использование переменной track_id для идентификатора объекта
            tracked_id = track_id

            # Увеличение track_id на 1 для каждого обнаруженного объекта
            track_id += 1

            # Добавление обнаруженного объекта в список для трекера
            output_dict["objects"].append(DetectedObject(xtl=xtl, ytl=ytl, xbr=xbr, ybr=ybr, class_name=class_name, tracked_id=tracked_id))
            detections.append(norfair.Detection(points=np.array([[xtl, ytl], [xbr, ybr]]), scores=np.array([1.0]), label=class_name, data=tracked_id))
            logger.info(f"Обнаружен объект: {class_name} с координатами: ({xtl}, {ytl}), ({xbr}, {ybr}) и идентификатором: {tracked_id}")'''
    tracked_objects = tracker.update(detections=detections)
    service_output = ServiceOutput(objects=output_dict["objects"])
    service_output_json = service_output.model_dump(mode="json")
    with open("output_json.json", "w") as output_file:
        json.dump(service_output_json, output_file, indent=4)
    return JSONResponse(content=jsonable_encoder(service_output_json))

uvicorn.run(app, host="localhost", port=7000)
