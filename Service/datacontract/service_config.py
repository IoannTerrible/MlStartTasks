import pydantic


class ServiceConfig(pydantic.BaseModel):
    name_of_classifier: str
    path_to_classifier: str
    name_of_detector: str
    path_to_detector: str

    """Целевая ширина изображения"""
    target_width: int
    """Целевая высота изображения"""
    target_height: int


