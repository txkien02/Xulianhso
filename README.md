# Xử lý ảnh và nhận diện biển số xe

Đọc file `Vehicle license plate recognition.pdf` để biết thêm lý thuyết.

## CÁC BƯỚC CHÍNH TRONG CỦA 1 BÀI TOÁN NHẬN DẠNG BIỂN SỐ XE

**Đây là các bước chính trong thuật toán nhận dạng biển số xe:**

1. Phát hiện biển số xe: Bước này nhằm tìm ra vị trí của biển số xe trong ảnh. Các phương pháp phổ biến để phát hiện biển số xe bao gồm sử dụng các thuật toán như Haar Cascade, HOG, YOLO, SSD, Faster R-CNN, RetinaNet, và EfficientDet.
2. Phân đoạn ký tự: Sau khi đã xác định được vị trí của biển số xe, bước tiếp theo là phân đoạn các ký tự trên biển số. Các phương pháp phân đoạn ký tự bao gồm sử dụng các thuật toán như Connected Component Analysis, Watershed Algorithm, và Deep Learning.
3. Nhận dạng ký tự: Cuối cùng, các ký tự trên biển số xe được nhận dạng bằng cách sử dụng các thuật toán như Template Matching, K-Nearest Neighbors, Support Vector Machines, Neural Networks, và Deep Learning. Các ký tự được nhận dạng được sắp xếp lại theo đúng thứ tự trên biển số xe để tạo thành biển số đầy đủ.

<p align="center"><img src="https://user-images.githubusercontent.com/40959407/130982072-a4701080-e40d-42c1-8fc5-062da340ca5b.png" width="300"></p>
<p align="center"><i>Figure 1. Đây là hình ảnh minh họa cho các bước chính trong thuật toán nhận dạng biển số xe</i></p>

## PHÁT HIỆN VÀ TÁCH BIỂN SỐ:

**Các bước chính trong phát hiện và trích xuất biển số xe**

Chụp ảnh từ camera
Chuyển ảnh sang ảnh xám
Tăng độ tương phản của ảnh
Giảm nhiễu bằng bộ lọc Gaussian
Áp dụng ngưỡng th adapt cho việc nhị phân hóa ảnh
Phát hiện biên Canny
Phát hiện biển số xe bằng cách vẽ đường viền và sử dụng câu lệnh if..else.

<p align="center"><img src="https://user-images.githubusercontent.com/40959407/130982322-86cd6ab1-c4de-48c2-b67a-3d52b75be330.jpg" width="300" ></p>
<p align="center"><i>Figure 2. Đây là các bước chính trong quá trình phát hiện và trích xuất biển số xe </i></p>

Đầu tiên từ clip ta sẽ cắt từng frame ảnh ra từ clip đầu vào để xử lý, tách biển số. Ở phạm vi đồ án này, ý tưởng chủ yếu là nhận diện được biển số từ sự thay đổi đột ngột về cường độ ánh sáng giữa biển số và môi trường xung quanh nên ta sẽ loại bỏ các dữ liệu màu sắc RGB bằng cách chuyển sang ảnh xám. Tiếp theo ta tăng độ tương phản với hai phép toán hình thái học Top Hat và Black Hat để làm nổi bật thêm biển số giữa phông nền, hỗ trợ cho việc xử lý nhị phân sau này. Sau đó, ta giảm nhiễu bằng bộ lọc Gauss để loại bỏ những chi tiết nhiễu có thể gây ảnh hưởng đến quá trình nhận diện, đồng thời làm tăng tốc độ xử lý.

Việc lấy ngưỡng sẽ giúp ta tách được thông tin biển số và thông tin nền, ở đây chọn lấy ngưỡng động (Adaptive Threshold). Tiếp đó ta sử dụng thuật toán phát hiện cạnh Canny để trích xuất những chi tiết cạnh của biển số. Trong quá trình xử lý máy tính có thể nhầm lẫn biển số với những chi tiết nhiễu, việc lọc lần cuối bằng các tỉ lệ cao/rộng hay diện tích của biển số sẽ giúp xác định được đúng biển số. Cuối cùng, ta sẽ xác định vị trí của biển số trong ảnh bằng cách vẽ Contour bao quanh.

## Phân tách kí tự:

Đầu tiên cần xoay biển số về đúng chính diện

To begin, we need to rotate the image to the right direction

Phương pháp xoay ảnh sử dụng ở đây là:

- Lọc ra tọa độ 2 đỉnh A,B nằm dưới cùng của biển số
- Từ 2 đỉnh có tọa độ lần lượt là A(x1, y1) và B(x2,y2) ta có thể tính được cạnh đối và cạnh kề của tam giác ABC
- Tính góc quay bằng hàm tan()
- Xoay ảnh theo góc quay đã tính. Nếu ngược lại điểm A nằm cao hơn điểm B ta cho góc quay âm

Từ ảnh nhị phân, ta lại tìm contour cho các kí tự (phần màu trắng). Sau đó vẽ những hình chữ nhật bao quanh các kí tự đó. Tuy nhiên việc tìm contour này cũng bị nhiễu dẫn đến việc máy xử lý sai mà tìm ra những hình ảnh không phải kí tự. Ta sẽ áp dụng các đặc điểm về tỉ lệ chiều cao/rộng của kí tự, diện tích của kí tự so với biển số

## Nhận dạng kí tự

EasyOCR được xây dựng với thư viện Deep Learning Python và Pytorch, có GPU có thể tăng tốc toàn bộ quá trình phát hiện. Phần phát hiện đang sử dụng thuật toán CRAFT và mô hình Nhận dạng là CRNN. Nó bao gồm 3 thành phần chính, trích xuất tính năng, ghi nhãn trình tự (LSTM) và giải mã (CTC). EasyOCR không có nhiều phụ thuộc vào phần mềm, nó có thể được sử dụng trực tiếp với API của nó. Nó cũng có thể được sử dụng như một thư viện độc lập trong các dự án khác.

## Nhận dạng kí tự

|  Category   | Total number of plates | Nunmber of found plates | Percentage(%) |
| :---------: | :--------------------: | :---------------------: | :-----------: |
| 1 row plate |          370           |           182           |     49,2      |
| 2 row plate |          2349          |           924           |     39,3      |

<p align="left"><i>Table 1. Tỷ lệ tìm được biển số xe trong hình </i></p>

Khi ta quay theo nhiều góc độ, nhiều vị trí dẫn đến khi tính toán diện tích, tỉ lệ cao/rộng của biển số không còn thỏa điều kiện đặt ra nên đã bị loại. Biển số có thể bị ảnh hưởng bởi những chi tiết ngoài nên khi xấp xỉ contour không ra hình tứ giác, dẫn đến cũng gây mất biển số. Lỗi này đặc biệt xảy ra ở những xe ô tô vì ô tô thường có nền xung quanh biển số là những vật liệu phản chiếu ánh sáng mạnh, gây ảnh hưởng lớn đến quá trình xác định vùng biển số.

Trong quá trình xử lý, việc xử lý nhị phân cũng đóng vai trò quan trọng, ảnh bị nhiễu và bản thân biển số bị tối, dính nhiều bụi dẫn đến khi xử lý nhị phân sẽ bị đứt đoạn và vẻ contour bị sai, để khắc phục cần sử dụng những phép toán hình thái học như phép nở, phép đóng để làm liền những đường màu trắng trong ảnh nhị phân.

|    Category    | Nunmber of found plates | 100% correctly recognizized | 1-character uncorrect | 2-character uncorrect | above 3-character uncorrect |
| :------------: | :---------------------: | :-------------------------: | :-------------------: | :-------------------: | :-------------------------: |
|  1 row plates  |           182           |             61              |          88           |          19           |             14              |
| Percentage (%) |           100           |            33,5             |         48,4          |         10,4          |             7,7             |

<p align="center"><i>Table 2. Tỷ lệ lỗi nhận dạng ký tự biển số xe 1 hàng</i></p>

|    Category    | Nunmber of found plates | 100% correctly recognizized | 1-character uncorrect | 2-character uncorrect | above 3-character uncorrect |
| :------------: | :---------------------: | :-------------------------: | :-------------------: | :-------------------: | :-------------------------: |
|  2 row plates  |           924           |             286             |          273          |          175          |             190             |
| Percentage (%) |           100           |             31              |         29,5          |         18,9          |            20,6             |

<p align="center"><i>Table 3. Tỷ lệ lỗi nhận dạng ký tự trên biển số xe 2 hàng</i></p>

Nhìn chung mô hình nhận diện KNN cũng khá tốt, có những kí tự dù bị mờ, bị nghiêng vẫn nhận diện đúng. Điều này một phần nhờ vào chương trình đã xoay biển số lại cho để tăng khả năng nhận diện, cho dù nghiêng thì kí tự cũng chỉ nghiêng từ 3° đến 7°. Tuy nhiên vẫn còn nhầm lẫn nhiều giữa các kí tự như số 1 với số 7. Chữ G, chữ D, số 6 với số 0. Chữ B với số 8...

### Ưu điểm

- Dễ cài đặt và sử dụng.
- Khá nhẹ nên máy tính với cấu hình yếu cũng có thể xử lý mượt mà so với các thuật toán khác như CNN, SVM.
- Phù hợp cho đối tượng sinh viên muốn tìm hiểu căn bản về xử lý ảnh hay trí tuệ nhân tạo.

### Khuyết điểm

- Khả năng nhận diện của KNN còn thấp, khi tập dữ liệu quá nhiều sẽ tăng thời gian xử lý vì phải quét hết tập dữ liệu train.
- Nhận diện kém với sự phản chiếu của biển số, sự di ảnh, chói sáng từ môi trường ngoài, những biển có phần chữ số không rõ ràng, với biển số xe ô tô

### Hướng phát triển

- Sử dụng camera chuyên dụng cho việc nhận diện biển số xe vì có khả năng chống chịu với sương mù, đêm tối, chói sáng...
- Sử dụng các thuật toán xử lý ảnh khác để xác định vị trí biển số tốt hơn như phương pháp biến đổi Hough để nhận diện đường thẳng, xác định bằng màu sắc, những thuật toán làm hạn chế sự di ảnh khi xe đang di chuyển.
