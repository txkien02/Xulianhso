import math
import cv2
import numpy as np

class LicensePlateRecognition:
    def __init__(self):
        self.ADAPTIVE_THRESH_BLOCK_SIZE = 19
        self.ADAPTIVE_THRESH_WEIGHT = 9
        self.Min_char = 0.01
        self.Max_char = 0.09
        self.RESIZED_IMAGE_WIDTH = 20
        self.RESIZED_IMAGE_HEIGHT = 30
        self.n = 1

    def load_knn_model(self):
        npaClassifications = np.loadtxt("classificationS.txt", np.float32)
        npaFlattenedImages = np.loadtxt("flattened_images.txt", np.float32)
        npaClassifications = npaClassifications.reshape((npaClassifications.size, 1))
        self.kNearest = cv2.ml.KNearest_create()
        self.kNearest.train(npaFlattenedImages, cv2.ml.ROW_SAMPLE, npaClassifications)

    def preprocess_image(self, img):
        img = cv2.resize(img, dsize=(1920, 1080))
        imgGrayscaleplate, imgThreshplate = Preprocess.preprocess(img)
        return img, imgGrayscaleplate, imgThreshplate

    def find_license_plate_contours(self, imgThreshplate):
        canny_image = cv2.Canny(imgThreshplate, 250, 255)
        kernel = np.ones((3, 3), np.uint8)
        dilated_image = cv2.dilate(canny_image, kernel, iterations=1)
        contours, hierarchy = cv2.findContours(dilated_image, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
        contours = sorted(contours, key=cv2.contourArea, reverse=True)[:10]
        return contours

    def process_license_plates(self, img, imgGrayscaleplate, imgThreshplate, contours):
        for screenCnt in self.get_screen_contours(contours):
            angle = self.find_license_plate_angle(screenCnt)
            roi, imgThresh = self.crop_license_plate(img, imgGrayscaleplate, imgThreshplate, screenCnt, angle)
            cont = self.find_character_contours(imgThresh)
            char_x = self.filter_characters(cont, roi)
            first_line, second_line = self.recognize_characters(char_x, cont, imgThresh, roi)
            self.display_result(img, roi, first_line, second_line)

    def get_screen_contours(self, contours):
        screenCnt = []
        for c in contours:
            peri = cv2.arcLength(c, True)
            approx = cv2.approxPolyDP(c, 0.06 * peri, True)
            [x, y, w, h] = cv2.boundingRect(approx.copy())
            ratio = w / h
            if len(approx) == 4:
                screenCnt.append(approx)
                cv2.putText(img, str(len(approx.copy())), (x, y), cv2.FONT_HERSHEY_DUPLEX, 2, (0, 255, 0), 3)
        return screenCnt

    def find_license_plate_angle(self, screenCnt):
        (x1, y1) = screenCnt[0, 0]
        (x2, y2) = screenCnt[1, 0]
        (x3, y3) = screenCnt[2, 0]
        (x4, y4) = screenCnt[3, 0]
        array = [[x1, y1], [x2, y2], [x3, y3], [x4, y4]]
        array.sort(reverse=True, key=lambda x: x[1])
        (x1, y1) = array[0]
        (x2, y2) = array[1]
        doi = abs(y1 - y2)
        ke = abs(x1 - x2)
        angle = math.atan(doi / ke) * (180.0 / math.pi)
        return angle

    def crop_license_plate(self, img, imgGrayscaleplate, imgThreshplate, screenCnt, angle):
        mask = np.zeros(imgGrayscaleplate.shape, np.uint8)
        new_image = cv2.drawContours(mask, [screenCnt], 0, 255, -1)
        (x, y) = np.where(mask == 255)
        (topx, topy) = (np.min(x), np.min(y))
        (bottomx, bottomy) = (np.max(x), np.max(y))
        roi = img[topx:bottomx, topy:bottomy]
        imgThresh = imgThreshplate[topx:bottomx, topy:bottomy]
        ptPlateCenter = (bottomx - topx) / 2, (bottomy - topy) / 2

        if x1 < x2:
            rotationMatrix = cv2.getRotationMatrix2D(ptPlateCenter, -angle, 1.0)
        else:
            rotationMatrix = cv2.getRotationMatrix2D(ptPlateCenter, angle, 1.0)

        roi = cv2.warpAffine(roi, rotationMatrix, (int(bottomy - topy), int(bottomx - topx)))
        imgThresh = cv2.warpAffine(imgThresh, rotationMatrix, (int(bottomy - topy), int(bottomx - topx)))
        roi = cv2.resize(roi, (0, 0), fx=3, fy=3)
        imgThresh = cv2.resize(imgThresh, (0, 0), fx=3, fy=3)

        return roi, imgThresh

    def find_character_contours(self, imgThresh):
        kerel3 = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))
        thre_mor = cv2.morphologyEx(imgThresh, cv2.MORPH_DILATE, kerel3)
        cont, hier = cv2.findContours(thre_mor, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        return cont

    def filter_characters(self, cont, roi):
        char_x_ind = {}
        char_x = []
        height, width, _ = roi.shape
        roiarea = height * width

        for ind, cnt in enumerate(cont):
            (x, y, w, h) = cv2.boundingRect(cont[ind])
            ratiochar = w / h
            char_area = w * h

            if (self.Min_char * roiarea < char_area < self.Max_char * roiarea) and (0.25 < ratiochar < 0.7):
                if x in char_x:
                    x = x + 1
                char_x.append(x)
                char_x_ind[x] = ind

        return sorted(char_x)

    def recognize_characters(self, char_x, cont, imgThresh, roi):
        strFinalString = ""
        first_line = ""
        second_line = ""
        for i in char_x:
            (x, y, w, h) = cv2.boundingRect(cont[char_x_ind[i]])
            cv2.rectangle(roi, (x, y), (x + w, y + h), (0, 255, 0), 2)
            imgROI = imgThresh[y:y + h, x:x + w]

            imgROIResized = cv2.resize(imgROI, (self.RESIZED_IMAGE_WIDTH, self.RESIZED_IMAGE_HEIGHT))
            npaROIResized = imgROIResized.reshape((1, self.RESIZED_IMAGE_WIDTH * self.RESIZED_IMAGE_HEIGHT))

            npaROIResized = np.float32(npaROIResized)
            _, npaResults, neigh_resp, dists = self.kNearest.findNearest(npaROIResized, k=3)
            strCurrentChar = str(chr(int(npaResults[0][0])))
            cv2.putText(roi, strCurrentChar, (x, y + 50), cv2.FONT_HERSHEY_DUPLEX, 2, (255, 255, 0), 3)

            if y < height / 3:
                first_line = first_line + strCurrentChar
            else:
                second_line = second_line + strCurrentChar

        return first_line, second_line, roi

    def process_image(self, image_path):
        img = cv2.imread(image_path)
        img = cv2.resize(img, dsize=(1920, 1080))
        imgGrayscaleplate, imgThreshplate = Preprocess.preprocess(img)
        canny_image = cv2.Canny(imgThreshplate, 250, 255)
        kernel = np.ones((3, 3), np.uint8)
        dilated_image = cv2.dilate(canny_image, kernel, iterations=1)
        contours, hierarchy = cv2.findContours(dilated_image, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
        contours = sorted(contours, key=cv2.contourArea, reverse=True)[:10]
        screenCnt = []

        for c in contours:
            peri = cv2.arcLength(c, True)
            approx = cv2.approxPolyDP(c, 0.06 * peri, True)
            [x, y, w, h] = cv2.boundingRect(approx.copy())
            ratio = w / h

            if len(approx) == 4:
                screenCnt.append(approx)
                cv2.putText(img, str(len(approx.copy())), (x, y), cv2.FONT_HERSHEY_DUPLEX, 2, (0, 255, 0), 3)

        if screenCnt is None:
            detected = 0
            print("No plate detected")
        else:
            detected = 1

        if detected == 1:
            n = 1

            for screenCnt in screenCnt:
                cv2.drawContours(img, [screenCnt], -1, (0, 255, 0), 3)
                angle = self.calculate_angle(screenCnt)
                roi, imgThresh = self.crop_license_plate(img, imgGrayscaleplate, imgThreshplate, screenCnt, angle)
                cont = self.find_character_contours(imgThresh)
                char_x = self.filter_characters(cont, roi)
                first_line, second_line, roi = self.recognize_characters(char_x, cont, imgThresh, roi)

                print("\nLicense Plate " + str(n) + " is: " + first_line + " - " + second_line + "\n")
                roi = cv2.resize(roi, None, fx=0.75, fy=0.75)
                cv2.imshow(str(n), cv2.cvtColor(roi, cv2.COLOR_BGR2RGB))

                n += 1

        cv2.imshow("img", img)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

# Main test function
if __name__ == "__main__":
    recognizer = LicensePlateRecognition()
    recognizer.load_knn_model()
    image_path = "path_to_image.jpg"  # Replace with the actual image path
    recognizer.process_image(image_path)
