import os
import cv2
import numpy as np
import matplotlib.pyplot as plt
import easyocr
import sys
import util

# Định nghĩa các hằng số
model_cfg_path = os.path.join('.', 'model', 'cfg', 'darknet-yolov3.cfg') # Đường dẫn đến file cấu hình của mô hình YOLOv3
model_weights_path = os.path.join('.', 'model', 'weights', 'model.weights') # Đường dẫn đến file trọng số của mô hình YOLOv3

input_dir = sys.argv[1]  # Lấy đường dẫn thư mục đầu vào từ tham số dòng lệnh

for img_name in os.listdir(input_dir):
    # Xóa danh sách độ chính xác và biển số xe trước mỗi ảnh
    accuracy_scores = []
    license_plate_list = []

    img_path = os.path.join(input_dir, img_name)  # Đường dẫn đến ảnh

    # Tải mô hình YOLOv3
    net = cv2.dnn.readNetFromDarknet(model_cfg_path, model_weights_path)

    # Đọc ảnh
    img = cv2.imread(img_path)
    H, W, _ = img.shape  # Kích thước ảnh (chiều cao, chiều rộng)

    # Chuyển đổi ảnh thành dạng blob để đưa vào mạng neural
    blob = cv2.dnn.blobFromImage(img, 1 / 255, (416, 416), (0, 0, 0), True)

    # Nhận dạng đối tượng trong ảnh bằng cách đưa blob vào mạng neural
    net.setInput(blob)
    detections = util.get_outputs(net)

    # Lưu trữ thông tin về bounding box, class ID và score của các đối tượng được nhận dạng
    bboxes = []
    class_ids = []
    scores = []

    for detection in detections:
        bbox = detection[:4]  # [x1, y1, x2, y2]
        xc, yc, w, h = bbox
        bbox = [int(xc * W), int(yc * H), int(w * W), int(h * H)]  # Chuyển đổi tọa độ về không gian ảnh gốc
        bbox_confidence = detection[4]
        class_id = np.argmax(detection[5:])
        score = np.amax(detection[5:])
        bboxes.append(bbox)
        class_ids.append(class_id)
        scores.append(score)

    # Áp dụng Non-Maximum Suppression (NMS) để loại bỏ các bounding box trùng lặp
    bboxes, class_ids, scores = util.NMS(bboxes, class_ids, scores)

    # Sử dụng EasyOCR để nhận dạng biển số xe và hiển thị thông tin
    reader = easyocr.Reader(['en'])
    for bbox_, bbox in enumerate(bboxes):
        xc, yc, w, h = bbox
        license_plate = img[int(yc - (h / 2)):int(yc + (h / 2)), int(xc - (w / 2)):int(xc + (w / 2)), :].copy()  # Cắt ảnh biển số
        img = cv2.rectangle(img, (int(xc - (w / 2)), int(yc - (h / 2))), (int(xc + (w / 2)), int(yc + (h / 2))),
                            (0, 255, 0), 15)  # Vẽ bounding box lên ảnh gốc
        license_plate_gray = cv2.cvtColor(license_plate, cv2.COLOR_BGR2GRAY)
        _, license_plate_thresh = cv2.threshold(license_plate_gray, 64, 255, cv2.THRESH_BINARY_INV)
        output = reader.readtext(license_plate_thresh)  # Nhận dạng thông tin trên biển số

        for out in output:
            text_bbox, text, text_score = out
            if text_score > 0.4:  # Lọc kết quả có độ chính xác lớn hơn ngưỡng
                print(text, text_score)
                license_plate_list.append(str(text))
                accuracy_scores.append(text_score)  # Lưu trữ độ chính xác vào danh sách

    # Hiển thị biểu đồ cuối cùng với độ chính xác
    plt.figure()
    plt.imshow(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))
    plt.title('License Plate Recognition')
    plt.axis('off')

    # Hiển thị thông tin độ chính xác và biển số lên biểu đồ
    for i, (score, plate) in enumerate(zip(accuracy_scores, license_plate_list)):
        y = 200 + i * 100  # Vị trí hiển thị trên trục y (có thể điều chỉnh)
        plt.text(10, y, f"Accuracy: {score:.2f} - Plate: {plate}", color='red', fontsize=10, fontweight='bold')

    plt.show()
