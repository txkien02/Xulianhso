import numpy as np
import cv2

# Hàm Non-Maximum Suppression (NMS) để loại bỏ các bounding box trùng lặp
def NMS(boxes, class_ids, confidences, overlapThresh=0.5):
    # Chuyển đổi thành mảng numpy
    boxes = np.asarray(boxes)
    class_ids = np.asarray(class_ids)
    confidences = np.asarray(confidences)

    # Trả về danh sách rỗng nếu không có bounding box nào
    if len(boxes) == 0:
        return [], [], []

    # Tính toán tọa độ các góc của bounding box
    x1 = boxes[:, 0] - (boxes[:, 2] / 2)
    y1 = boxes[:, 1] - (boxes[:, 3] / 2)
    x2 = boxes[:, 0] + (boxes[:, 2] / 2)
    y2 = boxes[:, 1] + (boxes[:, 3] / 2)

    # Tính toán diện tích của các bounding box
    areas = (x2 - x1 + 1) * (y2 - y1 + 1)

    indices = np.arange(len(x1))
    for i, box in enumerate(boxes):
        # Tạo các chỉ số tạm thời
        temp_indices = indices[indices != i]
        # Tìm tọa độ của bounding box giao nhau
        xx1 = np.maximum(box[0] - (box[2] / 2), boxes[temp_indices, 0] - (boxes[temp_indices, 2] / 2))
        yy1 = np.maximum(box[1] - (box[3] / 2), boxes[temp_indices, 1] - (boxes[temp_indices, 3] / 2))
        xx2 = np.minimum(box[0] + (box[2] / 2), boxes[temp_indices, 0] + (boxes[temp_indices, 2] / 2))
        yy2 = np.minimum(box[1] + (box[3] / 2), boxes[temp_indices, 1] + (boxes[temp_indices, 3] / 2))

        w = np.maximum(0, xx2 - xx1 + 1)
        h = np.maximum(0, yy2 - yy1 + 1)

        # Tính tỷ lệ giao nhau
        overlap = (w * h) / areas[temp_indices]
        # Nếu tỷ lệ giao nhau lớn hơn ngưỡng, loại bỏ bounding box
        if np.any(overlap) > overlapThresh:
            indices = indices[indices != i]

    # Trả về chỉ các bounding box còn lại
    return boxes[indices], class_ids[indices], confidences[indices]


# Hàm lấy kết quả từ mạng neural
def get_outputs(net):
    layer_names = net.getLayerNames()
    output_layers = [layer_names[i - 1] for i in net.getUnconnectedOutLayers()]
    outs = net.forward(output_layers)
    outs = [c for out in outs for c in out if c[4] > 0.1]
    return outs


# Hàm vẽ bounding box lên ảnh
def draw(bbox, img):
    xc, yc, w, h = bbox
    img = cv2.rectangle(img,
                        (xc - int(w / 2), yc - int(h / 2)),
                        (xc + int(w / 2), yc + int(h / 2)),
                        (0, 255, 0), 20)
    return img
