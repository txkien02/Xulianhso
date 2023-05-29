import math

import cv2
import numpy as np

import Preprocess

from typing import List, Tuple

class LicensePlateDetector:
    def __init__(self, classification_file: str, flattened_image_file: str):
        # Initialize constants for license plate detection
        self.ADAPTIVE_THRESH_BLOCK_SIZE = 19
        self.ADAPTIVE_THRESH_WEIGHT = 9
        self.Min_char_area = 0.015
        self.Max_char_area = 0.06
        self.Min_char = 0.01
        self.Max_char = 0.09
        self.Min_ratio_char = 0.25
        self.Max_ratio_char = 0.7
        self.max_size_plate = 18000
        self.min_size_plate = 5000
        self.RESIZED_IMAGE_WIDTH = 20
        self.RESIZED_IMAGE_HEIGHT = 30
        self.n = 1

        # Load the classification and flattened image files
        self.npaClassifications = np.loadtxt(classification_file, np.float32)
        self.npaFlattenedImages = np.loadtxt(flattened_image_file, np.float32)

        # Reshape the classifications array
        self.npaClassifications = self.npaClassifications.reshape((self.npaClassifications.size, 1))

        # Create a KNearest object and train it with the flattened images and classifications
        self.kNearest = cv2.ml.KNearest_create()
        self.kNearest.train(self.npaFlattenedImages, cv2.ml.ROW_SAMPLE, self.npaClassifications)

    def detect_license_plate(self, video_path: str) -> List[Tuple[Tuple[int, int, int, int], str]]:
        cap = cv2.VideoCapture(video_path)
        results = []

        while (cap.isOpened()):
            ret, img = cap.read()
            if not ret:
                break

            # Image preprocessing
            imgGrayscaleplate, imgThreshplate = Preprocess.preprocess(img)
            canny_image = cv2.Canny(imgThreshplate, 250, 255)  # Canny Edge
            kernel = np.ones((3, 3), np.uint8)
            dilated_image = cv2.dilate(canny_image, kernel,iterations=1)  # Dilation

                    # Filter out license plates
        contours, hierarchy = cv2.findContours(dilated_image, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
        contours = sorted(contours, key=cv2.contourArea, reverse=True)[:10]  # Pick out 10 biggest contours
        screenCnt = []
        for c in contours:
            peri = cv2.arcLength(c, True)  # Tính chu vi
            approx = cv2.approxPolyDP(c, 0.06 * peri, True)  # Approximate the edges of contours
            [x, y, w, h] = cv2.boundingRect(approx.copy())
            ratio = w / h
            if (len(approx) == 4) and (0.8 <= ratio <= 1.5 or 4.5 <= ratio <= 6.5):
                screenCnt.append(approx)
        if screenCnt is None:
            detected = 0
        else:
            detected = 1

        if detected == 1:
            for screenCnt in screenCnt:

                ################## Find the angle of the license plate ###############
                (x1, y1) = screenCnt[0, 0]
                (x2, y2) = screenCnt[1, 0]
                (x3, y3) = screenCnt[2, 0]
                (x4, y4) = screenCnt[3, 0]
                array = [[x1, y1], [x2, y2], [x3, y3], [x4, y4]]
                sorted_array = array.sort(reverse=True, key=lambda x: x[1])
                (x1, y1) = array[0]
                (x2, y2) = array[1]

                doi = abs(y1 - y2)
                ke = abs(x1 - x2)
                angle = math.atan(doi / ke) * (180.0 / math.pi)
                #################################################

                # Masking the part other than the number plate
                mask = np.zeros(imgGrayscaleplate.shape, np.uint8)
                new_image = cv2.drawContours(mask, [screenCnt], 0, 255, -1, )

                # Now crop
                (x, y) = np.where(mask == 255)
                (topx, topy) = (np.min(x), np.min(y))
                (bottomx, bottomy) = (np.max(x), np.max(y))

                roi = img[topx:bottomx + 1, topy:bottomy + 1]
                imgThresh = imgThreshplate[topx:bottomx + 1, topy:bottomy + 1]

                ptPlateCenter = (bottomx - topx) / 2, (bottomy - topy) / 2

                if x1 < x2:
                    rotationMatrix = cv2.getRotationMatrix2D(ptPlateCenter, -angle, 1.0)
                else:
                    rotationMatrix = cv2.getRotationMatrix2D(ptPlateCenter, angle, 1.0)

                roi = cv2.warpAffine(roi, rotationMatrix, (bottomy - topy, bottomx - topx))
                imgThresh = cv2.warpAffine(imgThresh, rotationMatrix, (bottomy - topy, bottomx - topx))

                roi = cv2.resize(roi, (0, 0), fx=3, fy=3)
                imgThresh = cv2.resize(imgThresh, (0, 0), fx=3, fy=3)

                # License Plate preprocessing
                kerel3 = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))
                thre_mor = cv2.morphologyEx(imgThresh, cv2.MORPH_DILATE, kerel3)
                cont, hier = cv2.findContours(thre_mor, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

                # Character segmentation
                char_x_ind = {}
                char_x = []
                height, width, _ = roi.shape
                roiarea = height * width
                for ind, cnt in enumerate(cont):
                    area = cv2.contourArea(cnt)
                    (x, y, w, h) = cv2.boundingRect(cont[ind])
                    ratiochar = w / h
                    if (self.Min_char * roiarea < area < self.Max_char * roiarea) and (self.Min_ratio_char < ratiochar < self.Max_ratio_char):
                        if x in char_x:  
                            x = x + 1
                        char_x.append(x)
                        char_x_ind[x] = ind

                # Character recognition
                if len(char_x) in range(7, 10):
                    cv2.drawContours(img, [screenCnt], -1, (0, 255, 0), 3)

                    char_x = sorted(char_x)
                    strFinalString = ""
                    first_line = ""
                    second_line = ""

                    for i in char_x:
                        (x, y, w, h) = cv2.boundingRect(cont[char_x_ind[i]])
                        cv2.rectangle(roi, (x, y), (x + w, y + h), (0, 255, 0), 2)

                        imgROI = thre_mor[y:y + h, x:x + w] 

                        imgROIResized = cv2.resize(imgROI,
                                                   (self.RESIZED_IMAGE_WIDTH, self.RESIZED_IMAGE_HEIGHT))  
                        npaROIResized = imgROIResized.reshape(
                            (1, self.RESIZED_IMAGE_WIDTH * self.RESIZED_IMAGE_HEIGHT))  
                        npaROIResized = np.float32(npaROIResized)  
                        _, npaResults, neigh_resp, dists = self.kNearest.findNearest(npaROIResized,k=3)  
                        strCurrentChar = str(chr(int(npaResults[0][0])))  

                        if (y < height / 3):   
                            first_line = first_line + strCurrentChar
                        else:
                            second_line = second_line + strCurrentChar

                    strFinalString = first_line + second_line
                    results.append(((topy, topx, bottomy - topy, bottomx - topx), strFinalString))

            cap.release()
   
        return results
    
    def detect(self, img):
        results = []
        # Convert the input image to grayscale
        imgGrayscale = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        # Apply Gaussian blur to the grayscale image
        imgGrayscale = cv2.GaussianBlur(imgGrayscale, (5, 5), 0)
        # Apply adaptive thresholding to the blurred image
        imgThresh = cv2.adaptiveThreshold(imgGrayscale, 255.0, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY_INV,
                                          19, 9)
        # Invert the thresholded image
        imgThresh = cv2.bitwise_not(imgThresh)
        # Apply median blur to the inverted image
        imgThresh = cv2.medianBlur(imgThresh, 5)
        # Resize the thresholded image and the input image by a factor of 3
        imgThresh = cv2.resize(imgThresh, (0, 0), fx=3, fy=3)
        img = cv2.resize(img, (0, 0), fx=3, fy=3)
        # Define a 3x3 kernel for morphological dilation
        kerel3 = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))
        # Apply morphological dilation to the thresholded image
        thre_mor = cv2.morphologyEx(imgThresh, cv2.MORPH_DILATE, kerel3)
        # Find contours in the dilated image
        cont, hier = cv2.findContours(thre_mor, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        # Loop through the contours and check if they meet the criteria for a license plate
        for ind, cnt in enumerate(cont):
            area = cv2.contourArea(cnt)
            (x, y, w, h) = cv2.boundingRect(cont[ind])
            ratio = w / h
            if (self.Min_plate * imgThresh.shape[0] * imgThresh.shape[1] < area < self.Max_plate * imgThresh.shape[0] * imgThresh.shape[1]) and (self.Min_ratio_plate < ratio < self.Max_ratio_plate):
                # If the contour meets the criteria, extract the region of interest (ROI) and the thresholded plate image
                roi = img[y:y + h, x:x + w]
                imgThreshplate = imgThresh[y:y + h, x:x + w]
                # Pass the ROI and thresholded plate image to the plate_detection method for further processing
                results = self.plate_detection(roi, imgThreshplate)
                break

        # Return the results of the plate_detection method
        return results
