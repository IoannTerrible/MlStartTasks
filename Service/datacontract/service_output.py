import pydantic
from typing import List

class DetectedObject(pydantic.BaseModel):
    xtl: int
    ytl: int
    xbr: int
    ybr: int
    class_name: str
    tracked_id: int

# класс(-ы), описывающий выход сервиса
class ServiceOutput(pydantic.BaseModel):
    """Ширина преобразованного изображения"""
    width: int = pydantic.Field(default=640)
    """Высота преобразованного изображения"""
    height: int = pydantic.Field(default=480)
    """Число каналов преобразованного изображения"""
    channels: int = pydantic.Field(default=3)

    objects: List[DetectedObject]