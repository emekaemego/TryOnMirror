# TryOn Mirror

> Note; This was a personal project I worked on in 2013. This GitHub repo is for reference purposes only. I can't guarantee the code still works. Some files have been removed for security and copyright reasons.

## What It's About

TryOn Mirror offers users an advanced virtual makeover technology; making the online makeover experience more like a beauty counter and salon experience than ever before. TryOn Mirror enables you to virtually try-on different cosmetic brands, accessories, hairstyles and colors on your photo.

!["Tryon Mirror screenshot"](/TryOnMirror.UI.Web/assets/images/screenshots.png?raw=true)

!["Tryon Mirror screenshot"](/TryOnMirror.UI.Web/assets/images/makeover_6a7ea41c_3dfe_4727_a946_e33a181390d5.jpg?raw=true)

## Technical Info

TryOn Mirror is a virtual makeover web-based application created with ASP.NET MVC, JavaScript (jQuery, kinetic.js, etc.), OpenCV, Emgu CV.

I used KineticJs for HTML5 Canvas to archive the photo calibration and virtual makeover augmentation interactivity.

### Face and Facial Feature Detection

After a user successfully uploads a photo, the system tries detecting the face and other facial features, like eyes, nose, lips, iris and the face shape in the uploaded image.

!["Detected face region ready for user calibration if needed"](/TryOnMirror.UI.Web/assets/images/trace-tip-face.jpg?raw=true)

### Calibration

<!--After the face and features detection process, the user is taken to a page where they will calibrate the detected regions. If the system was unable to detect the face, the system makes a calculated guess on the face and facial feature positions. -->

The user is aided with a visual guide for the image calibration, as shown in the images below. They drag the red dots on to adjust that particular region.

!["Facial feature detected regions"](/TryOnMirror.UI.Web/assets/images/tryon-mirror-collage.jpg?raw=true)
