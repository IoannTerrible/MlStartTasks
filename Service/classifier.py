import torch
from torchvision import transforms

class Classifier:
    def __init__(self, model_path, class_names):
        self.model = torch.load(model_path, map_location=torch.device('cpu'))
        self.model.eval()
        self.class_names = class_names
        self.transform = transforms.Compose([
            transforms.Resize(256),
            transforms.CenterCrop(224),
            transforms.ToTensor(),
            transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])
        ])

    def classify(self, image):
        tensor_image = self.transform(image).unsqueeze(0)
        with torch.no_grad():
            output = self.model(tensor_image)
        _, predicted_idx = torch.max(output, 1)
        class_index = predicted_idx.item()
        return self.class_names[class_index]