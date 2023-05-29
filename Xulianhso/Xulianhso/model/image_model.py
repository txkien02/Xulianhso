import cv2
import math
import numpy as np
import Preprocess

from typing import List, Tuple

class LicensePlateDetector:
    def __init__(self, classification_file: str, flattened_image_file: str):
        self.ADAPTIVE_THRESH_BLOCK_SIZE = 19
        self.ADAPTIVE_THRESH_WEIGHT = 9
        self.Min_char = 0.01
        self.Max_char = 0.09

        self.RESIZED_IMAGE_WIDTH = 20
        self.RESIZED_IMAGE_HEIGHT = 30
        self.n = 1

        self.npaClassifications = np.loadtxt(classification_file, np.float32)
        self.npaFlattenedImages = np.loadtxt(flattened_image_file, np.float32)
        self.npaClassifications = self.npaClassifications.reshape(
            (self.npaClassifications.size, 1))
        self.kNearest = cv2.ml.KNearest_create()
        self.kNearest.train(self.npaFlattenedImages,
                             cv2.ml.ROW_SAMPLE, self.npaClassifications)

    def __find_plate(self, img):
        detected = 0
        img = cv2.resize(img, dsize=(1920, 1080))

        imgGrayscaleplate, imgThreshplate = Preprocess.preprocess(img)
        canny_image = cv2.Canny(imgThreshplate, 250, 255)
        kernel = np.ones((3, 3), np.uint8)
        dilated_image = cv2.dilate(canny_image, kernel, iterations=1)

        contours, hierarchy = cv2.findContours(dilated_image,
                                                cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
        contours = sorted(contours, key=cv2.contourArea, reverse=True)[:10]

        screenCnt = []
        for c in contours:
            peri = cv2.arcLength(c, True)
            approx = cv2.approxPolyDP(c, 0.06 * peri, True)
            [x, y, w, h] = cv2.boundingRect(approx.copy())
            ratio = w / h
            if (len(approx) == 4) and (ratio > 0.8) and (w > 100):
                screenCnt.append(approx)

        if len(screenCnt) == 0:
            return []

        results = []
        for screen in screenCnt:
            (x1, y1) = screen[0, 0]
            (x2, y2) = screen[1, 0]
            (x3, y3) = screen[2, 0]
            (x4, y4) = screen[3, 0]

            if y1 > y2:
                angle = math.atan(abs(y1 - y2) / abs(x1 - x2)) * (
                    180.0 / math.pi)
            else:
                angle = math.atan(abs(y2 - y1) / abs(x1 - x2)) * (
                    180.0 / math.pi)

            ptCenter = ((x1+x2+x3+x4) / 4, (y1+y2+y3+y4) / 4)
            height, width, _ = img.shape

            rotationMatrix = cv2.getRotationMatrix2D(
                ptCenter, -angle, 1.0)
            imgRotated = cv2.warpAffine(
                img, rotationMatrix, (width, height))

            imgCropped = cv2.getRectSubPix(imgRotated, (int(
                width/2), int(height/2)), ptCenter)
            imgCropped = cv2.resize(imgCropped, (0, 0), fx=3, fy=3)
            imgThreshold = cv2.adaptiveThreshold(
                cv2.cvtColor(imgCropped, cv2.COLOR_BGR2GRAY), 250, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY_INV, self.ADAPTIVE_THRESH_BLOCK_SIZE, self.ADAPTIVE_THRESH_WEIGHT)

            contours, hierarchy = cv2.findContours(imgThreshold,
                                                    cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

            imgContours = np.zeros(imgThreshold.shape, np.uint8)
            cv2.drawContours(imgContours, contours, -1, (255, 255, 255), -1)

            if imgContours.any():
                x, y, w, h = cv2.boundingRect(contours[0])
                imgROI = imgContours[y:y+h, x:x+w]
                imgROIResized = cv2.resize(
                    imgROI, (self.RESIZED_IMAGE_WIDTH, self.RESIZED_IMAGE_HEIGHT))
                npaROIResized = imgROIResized.reshape(
                    (1, self.RESIZED_IMAGE_WIDTH * self.RESIZED_IMAGE_HEIGHT))
                npaROIResized = np.float32(npaROIResized)

                _, npaResults, _, _ = self.kNearest.findNearest(
                    npaROIResized, k=3)

                char = chr(int(npaResults[0][0]))
                results.append((char, (x, y, w, h)))

        return results

    def detect_license_plate(self, img_path: str) -> List[Tuple[Tuple[int, int, int, int], str]]:
        img = cv2.imread(img_path)
        results = self.__find_plate(img)

        return results

