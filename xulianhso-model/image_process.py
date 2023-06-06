import os
import cv2
import numpy as np
import matplotlib.pyplot as plt
import easyocr
import sys
import util

# Định nghĩa các hằng số
model_cfg_path = os.path.join('.', 'model', 'cfg', 'darknet-yolov3.cfg')
model_weights_path = os.path.join('.', 'model', 'weights', 'model.weights')
class_names_path = os.path.join('.', 'model', 'class', 'class.names')

input_dir = sys.argv[1]

for img_name in os.listdir(input_dir):
    # Xóa danh sách độ chính xác
    accuracy_scores = []
    license_plate_list = []

    img_path = os.path.join(input_dir, img_name)

    # Đọc tên lớp
    with open(class_names_path, 'r') as f:
        class_names = [j[:-1] for j in f.readlines() if len(j) > 2]

    # Tải mô hình
    net = cv2.dnn.readNetFromDarknet(model_cfg_path, model_weights_path)

    # Đọc ảnh
    img = cv2.imread(img_path)
    H, W, _ = img.shape

    # Chuyển đổi ảnh
    blob = cv2.dnn.blobFromImage(img, 1 / 255, (416, 416), (0, 0, 0), True)

    # Nhận dạng
    net.setInput(blob)
    detections = util.get_outputs(net)

    # Lưu trữ thông tin bounding boxes, class IDs và scores
    bboxes = []
    class_ids = []
    scores = []

    for detection in detections:
        bbox = detection[:4]  # [x1, x2, x3, x4]
        xc, yc, w, h = bbox
        bbox = [int(xc * W), int(yc * H), int(w * W), int(h * H)]
        bbox_confidence = detection[4]
        class_id = np.argmax(detection[5:])
        score = np.amax(detection[5:])
        bboxes.append(bbox)
        class_ids.append(class_id)
        scores.append(score)

    # Áp dụng Non-Maximum Suppression (NMS)
    bboxes, class_ids, scores = util.NMS(bboxes, class_ids, scores)

    
    # Nhận dạng biển số xe và hiển thị thông tin
    reader = easyocr.Reader(['en'])
    for bbox_, bbox in enumerate(bboxes):
        xc, yc, w, h = bbox
        license_plate = img[int(yc - (h / 2)):int(yc + (h / 2)), int(xc - (w / 2)):int(xc + (w / 2)), :].copy()
        img = cv2.rectangle(img, (int(xc - (w / 2)), int(yc - (h / 2))), (int(xc + (w / 2)), int(yc + (h / 2))),
                            (0, 255, 0), 15)
        license_plate_gray = cv2.cvtColor(license_plate, cv2.COLOR_BGR2GRAY)
        _, license_plate_thresh = cv2.threshold(license_plate_gray, 64, 255, cv2.THRESH_BINARY_INV)
        output = reader.readtext(license_plate_thresh)

        for out in output:
            text_bbox, text, text_score = out
            if text_score > 0.4:
                print(text, text_score)
                license_plate_list.append(str(text))
                accuracy_scores.append(text_score)  # Thêm độ chính xác vào danh sách

    # Hiển thị biểu đồ cuối cùng với độ chính xác
    plt.figure()
    plt.imshow(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))
    plt.title('License Plate Recognition')
    plt.axis('off')

    # Hiển thị thông tin độ chính xác lên biểu đồ
    for i, (score, plate) in enumerate(zip(accuracy_scores, license_plate_list)):
        y = 200 + i * 100  # Thay đổi giá trị 70 để cách nhau nhiều hơn
        plt.text(10, y, f"Accuracy: {score:.2f} - Plate: {plate}", color='red', fontsize=10, fontweight='bold')

    plt.show()
