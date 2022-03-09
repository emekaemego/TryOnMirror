Kinetic.Image.prototype.createImageHitRegion = function(callback, opacity) {
    //var config = newConfig || { };
    var that = this,
        width = this.getWidth(),
        height = this.getHeight(),
        canvas = new Kinetic.Canvas({
            width: width,
            height: height
        }),
        context = canvas.getContext(),
        image = this.getImage(),
        imageData, data, rgbColorKey, i, n;

    context.drawImage(image, 0, 0);

    //check if opacity is boolean
    if (typeof opacity == 'number') {
        //recalc opacity to hexvalue
        if (opacity <= 0) {
            opacity = 0;
        } else if (opacity >= 100) {
            opacity = 255;
        } else {
            opacity = Math.round(opacity * 2.55);
        }

        //check if boolean
    } else if (typeof opacity == 'boolean') {
        if (opacity) {
            opacity = 0;
        } else {
            opacity = 255;
        }
        //default value
    } else {
        opacity = 255;
    }


    try {
        imageData = context.getImageData(0, 0, width, height);
        data = imageData.data;
        rgbColorKey = Kinetic.Util._hexToRgb(this.colorKey);

        // replace non transparent pixels with color key
        for (i = 0, n = data.length; i < n; i += 4) {
            if (data[i + 3] > opacity) {
                data[i] = rgbColorKey.r;
                data[i + 1] = rgbColorKey.g;
                data[i + 2] = rgbColorKey.b;
                data[i + 3] = 255;
            }
        }
        
        Kinetic.Util._getImage(imageData, function(imageObj) {
            that.imageHitRegion = imageObj;
            if (callback) {
                callback();
            }
        });
    } catch(e) {
        Kinetic.Util.warn('Unable to create image hit region. ' + e.message);
        //console.log('Unable to create image hit region. ' + e.message);
    }

};

Kinetic.Rect.prototype.getDimension = function() {
    var pos = this.getPosition();
    var size = this.getSize();

    var dim = {
        x: pos.x,
        y: pos.y,
        width: size.width,
        height: size.height,
        left: pos.x,
        right: pos.x + size.width,
        top: pos.y,
        bottom: pos.y + size.height
    };

    return dim;
};

Kinetic.Image.prototype.getDimension = function () {
    var pos = this.getPosition();
    var size = this.getSize();

    var dim = {
        x: pos.x,
        y: pos.y,
        width: size.width,
        height: size.height,
        left: pos.x,
        right: pos.x + size.width,
        top: pos.y,
        bottom: pos.y + size.height
    };

    return dim;
};

"use strict";

var VirtualMakeover = {};

VirtualMakeover.Util = {
    //fearturesId: {
    //    faceId: "face",
    //    leftEyeId: "leftEye",
    //    rightEyeId: "rightEye",
    //    eyeBrowsId: "leftEyeBrow",
    //    leftEyeBrowId: "leftEyeBrow",
    //    rightEyeBrowId: "rightEyeBrow",
    //    leftEyeballId: "leftEyeball",
    //    rightEyeballId: "rightEyeball",
    //    mouthId: "lips",
    //    innerMouthId: "openLips",
    //    noseId: "nose",
    //},
    ui: {
        stage: null,
        scale: 1,
        zoomFactor: 1.1,
        origin: {
            x: 0,
            y: 0
        },
        zoom: function(event) {
            event.preventDefault();
            var evt = event.originalEvent,
                mx = evt.clientX /* - canvas.offsetLeft */,
                my = evt.clientY /* - canvas.offsetTop */,
                wheel = evt.wheelDelta / 120;
            var zoom = (ui.zoomFactor - (evt.wheelDelta < 0 ? 0.2 : 0));
            var newscale = ui.scale * zoom;
            ui.origin.x = mx / ui.scale + ui.origin.x - mx / newscale;
            ui.origin.y = my / ui.scale + ui.origin.y - my / newscale;

            ui.stage.setOffset(ui.origin.x, ui.origin.y);
            ui.stage.setScale(newscale);
            ui.stage.draw();

            ui.scale *= zoom;
        }
    },

    //zoom: function (roi, stage) {
    //    var zoomFactor = 1.1;
    //    scale = 1;
    //},

    isEmpty: function(value) {
        var result = (value == null || value.length === '');

        if (!result && $.isArray(value)) {
            result = (value.length == 0);
        }

        return result;
    },

    anchorsToPoints: function(anchors) {

        var result = [];

        for (var i = 0; i < anchors.length; i++) {
            result.push({ 'x': anchors[i].attrs.x, 'y': anchors[i].attrs.y });
        }

        return result;
    },

    pointsToRect: function(points) {

        var minX = $.Enumerable.From(points).Min(function(o) { return o.x; });

        var minY = $.Enumerable.From(points).Min(function(o) { return o.y; });
        var maxX = $.Enumerable.From(points).Max(function(o) { return o.x; });
        var maxY = $.Enumerable.From(points).Max(function(o) { return o.y; });

        var width = maxX - minX;
        var height = maxY - minY;

        var rect = Rect(minX, minY, width, height);

        return rect;
    },

    anchorsToRect: function(anchors) {
        var points = VirtualMakeover.Util.anchorsToPoints(anchors);

        var rect = this.pointsToRect(points);

        return rect;
    },

    getRotateRadians: function(startPoint, endPoint) {
        var radians = Math.atan2(endPoint.y - startPoint.y, endPoint.x - startPoint.x);

        return radians;
    },

    getRotateDegrees: function(startPoint, endPoint) {
        var degrees = Math.atan2(endPoint.y - startPoint.y, endPoint.x - startPoint.x) * (180 / Math.PI);

        return degrees;
    },

    zoom: function(roi, stage) {

        stage.setOffset(0, 0); //reset stage offset
        //this.layer.setPosition(0, 0); //reset mainLayer's position incase it was dragged
        stage.setScale(1); //reset stage scale
        stage.draw();

        var scale = 0;

        var offsetX, offsetY;

        offsetX = stage.getPosition().x; //roi.x;
        offsetY = stage.getPosition().y; //roi.y;

        if (roi.height > roi.width) {
            scale = (stage.getHeight() / roi.height);
        } else {
            scale = (stage.getWidth() / roi.width);
        }

        scale = scale > 3.5 ? 3.5 : scale;

        stage.setOffset(offsetX, offsetY);
        stage.setScale(scale);
        stage.draw();
    },

    clone: function(object) {
        jQuery.extend({
            deepClone: function(objThing) {
                // return jQuery.extend(true, {}, objThing);
                /// Fix for arrays, without this, arrays passed in are returned as OBJECTS! WTF?!?!
                if (jQuery.isArray(objThing)) {
                    return jQuery.makeArray(jQuery.deepClone($(objThing)));
                }
                return jQuery.extend(true, { }, objThing);
            },
        });

        return $.deepClone(object);
    },

    Rect: function(x, y, width, height) {

        var rect = {
            x: x,
            y: y,
            width: width,
            height: height,
            top: y,
            bottom: y + height,
            right: x + width,
            left: x,
        };

        return rect;
    },

    addPhotoPositionToCoords: function(imageX, imageY, coords) {

        var points = this.clone(coords);

        for (var i = 0; i < points.length; i++) {
            points[i].x = points[i].x + imageX;
            points[i].y = points[i].y + imageY;
        }

        return coords;
    },

    notify: function(callback, data) {
        if (typeof(callback) == "function") {
            callback(data);
        }
    },

    findByPropValue: function(array, propName, propValue) {
        //return $.grep(array, function(n, i){
        //    return n.purpose == purposeName;
        //});
        return $.grep(array, function(e) { return e[propName] === propValue; });
    },
    
    urlParam: function (url, name) {
        var results = new RegExp('[\\?&#]' + name + '=([^&#]*)').exec(url);
        if (!results) {
            return '';
        }
        return results[1] || '';
    }
};

VirtualMakeover.Main = function(options) {
    //public properties
    //this.eyesManualTraceAnchors = [];
    //this.mouthManualTraceAnchors = [];
    //this.noseManualTraceAnchors = [];
    this.openLips = false;

    //private properties
    var faceId = "face";
    var leftEyeId = "leftEye";
    var rightEyeId = "rightEye";
    //var eyeBrowsId = "leftEyeBrow";
    var leftEyeBrowId = "leftEyeBrow";
    var rightEyeBrowId = "rightEyeBrow";
    var leftEyeballId = "leftEyeball";
    var rightEyeballId = "rightEyeball";
    var glassId = "glass";
    var mouthId = "lips";
    var openLipsId = "openLips";
    var noseId = "nose";
    var lastActiveBlobAnchor = null;
    var traceAmountClicked = 0;
    var util = VirtualMakeover.Util;
    var main = this;
    //var _this = this;
    //var scale = 1;

    var updateCalibrationImageResize = function(activeAnchor) {

        var group = activeAnchor.getParent();

        var topLeft = group.get('.topLeft')[0];
        var topRight = group.get('.topRight')[0];
        var bottomRight = group.get('.bottomRight')[0];
        var bottomLeft = group.get('.bottomLeft')[0];
        var image = group.get('.image')[0];

        var anchorX = activeAnchor.getX();
        var anchorY = activeAnchor.getY();

        // update anchor positions
        switch (activeAnchor.getName()) {
        case 'topLeft':
            topRight.setY(anchorY);
            bottomLeft.setX(anchorX);
            break;
        case 'topRight':
            topLeft.setY(anchorY);
            bottomRight.setX(anchorX);
            break;
        case 'bottomRight':
            bottomLeft.setY(anchorY);
            topRight.setX(anchorX);
            break;
        case 'bottomLeft':
            bottomRight.setY(anchorY);
            topLeft.setX(anchorX);
            break;
        }

        image.setPosition(topLeft.getPosition());

        var width = topRight.getX() - topLeft.getX();
        var height = bottomLeft.getY() - topLeft.getY();
        if (width && height) {
            image.setSize(width, height);
        }
    };

    //Add anchor for resizing calibration image
    var addCalibrationAnchor = function(group, x, y, name) {

        //var stage = group.getStage();
        var layer = group.getLayer();

        var anchor = new Kinetic.Circle({
            x: x,
            y: y,
            stroke: '#00ff',
            fill: 'red',
            strokeWidth: 1,
            radius: 5,
            name: name,
            draggable: true,
            dragOnTop: false,
            strokeScaleEnabled: false,
            anchor: true
        });
        
        anchor.on('dragmove', function() {
            updateItemImagePosition(this);
            layer.draw();
        });

        //anchor.on('mousedown touchstart', function() {
        //    //group.setDraggable(false);
        //    this.moveToTop();
        //});

        //anchor.on('dragend', function() {
        //    group.setDraggable(true);
        //    layer.draw();
        //});
        // add hover styling
        anchor.on('mouseover', function() {
            //var layer = this.getLayer();
            document.body.style.cursor = 'pointer';
            //this.setStrokeWidth(4);
            layer.draw();
        });

        anchor.on('mouseout', function() {
            //var layer = this.getLayer();
            document.body.style.cursor = 'default';
            // this.setStrokeWidth(2);
            layer.draw();
        });

        group.add(anchor);
    };
    
    var initItemImageAnchors = function (image) {

        var group = image.getParent();
        var imgPos = image.getPosition();
        var imgSize = image.getSize();
        var rect = Rect(imgPos.x, imgPos.y, imgSize.width, imgSize.height);

        addCalibrationAnchor(group, rect.left, rect.top, 'topLeft');
        addCalibrationAnchor(group, rect.right, rect.top, 'topRight');
        addCalibrationAnchor(group, rect.right, rect.bottom, 'bottomRight');
        addCalibrationAnchor(group, rect.left, rect.bottom, 'bottomLeft');

        image.moveToBottom();

        group.getLayer().draw();
    };

    var updateItemImagePosition = function (caller) {

        var group = caller.getParent();
        var topLeft = group.get('.topLeft')[0];
        var topRight = group.get('.topRight')[0];
        var bottomRight = group.get('.bottomRight')[0];
        var bottomLeft = group.get('.bottomLeft')[0];
        var image = group.get('.image')[0];
        var activeHandleName = caller.getName(),
            newWidth,
            newHeight,
            imageX,
            imageY;

        if (caller.className === 'Image') {

            var imgPos = image.getPosition();
            var imgSize = image.getSize();
            var rect = Rect(imgPos.x, imgPos.y, imgSize.width, imgSize.height);

            topLeft.setPosition(rect.left, rect.top);
            topRight.setPosition(rect.right, rect.top);
            bottomRight.setPosition(rect.right, rect.bottom);
            bottomLeft.setPosition(rect.left, rect.bottom);

        } else {

            if (caller.className === 'Circle') {
                var anchorX = caller.getX();
                var anchorY = caller.getY();

                // update anchor positions
                switch (caller.getName()) {
                    case 'topLeft':
                        topRight.setY(anchorY);
                        bottomLeft.setX(anchorX);
                        break;
                    case 'topRight':
                        topLeft.setY(anchorY);
                        bottomRight.setX(anchorX);
                        break;
                    case 'bottomRight':
                        bottomLeft.setY(anchorY);
                        topRight.setX(anchorX);
                        break;
                    case 'bottomLeft':
                        bottomRight.setY(anchorY);
                        topLeft.setX(anchorX);
                        break;
                }
            }

            image.setPosition(topLeft.getPosition());

            var width = topRight.getX() - topLeft.getX();
            var height = bottomLeft.getY() - topLeft.getY();

            if (width && height) {
                image.setSize(width, height);
            }
            
            //newHeight = bottomLeft.getY() - topLeft.getY();
            //newWidth = image.getWidth() * newHeight / image.getHeight();

            //// Move the image to adjust for the new dimensions.
            //// The position calculation changes depending on where it is anchored.
            //// ie. When dragging on the right, it is anchored to the top left,
            ////     when dragging on the left, it is anchored to the top right.
            //if (activeHandleName === "topRight" || activeHandleName === "bottomRight") {
            //    image.setPosition(topLeft.getX(), topLeft.getY());
            //} else if (activeHandleName === "topLeft" || activeHandleName === "bottomLeft") {
            //    image.setPosition(topRight.getX() - newWidth, topRight.getY());
            //}

            //imageX = image.getX();
            //imageY = image.getY();

            //// Update handle positions to reflect new image dimensions
            //topLeft.setPosition(imageX, imageY);
            //topRight.setPosition(imageX + newWidth, imageY);
            //bottomRight.setPosition(imageX + newWidth, imageY + newHeight);
            //bottomLeft.setPosition(imageX, imageY + newHeight);

            //// Set the image's size to the newly calculated dimensions
            //if (newWidth && newHeight) {
            //    image.setSize(newWidth, newHeight);
            //}
        }
    };
    
    var initNewItemImage = function (src, id, group, callback) {
        var layer = group.getLayer();

        var imgObj = new Image();

        imgObj.onload = function () {

            var ratio = 1;
            var maxWidth = main.stage.getWidth();
            var maxHeight = main.stage.getHeight();
            var width = imgObj.width;
            var height = imgObj.height;

            if (width > maxWidth && width > height) {
                ratio = maxWidth / width;
            } else if (height > maxHeight) {
                ratio = maxHeight / height;
            }
            width = width * ratio;
            height = height * ratio;

            var x = (maxWidth - width) / 2;
            var y = (maxHeight - height) / 2;

            var image = layer.get('#' + id)[0];

            if (util.isEmpty(image)) {
                image = new Kinetic.Image({
                    x: x,
                    y: y,
                    width: width,
                    height: height,
                    image: imgObj,
                    id: id,
                    name: 'image',
                    draggable: true,
                    originalImage: imgObj
                });

                group.add(image);

            } else {
                image.setPosition(x, y);
                image.setSize(width, height);
            }

            //image.createImageHitRegion(function () { layer.draw(); }, 90);

            util.notify(callback, image);
        };

        imgObj.src = src;
    };
    
    var initNewCalibrationItemImage = function (src, id, group, callback) {
        var layer = group.getLayer();
        
        initNewItemImage(src, id, group, function (image) {
            
            initItemImageAnchors(image);

            image.on('dragmove', function () {
                updateItemImagePosition(this);
                layer.draw();
            });
            
            util.notify(callback, image);
        });
    };

    var createTraceRect = function (id, group, points) {
        var createAnchor = function (x, y, name) {

            var anchor = new Kinetic.Circle({
                x: x,
                y: y,
                stroke: '#00ff',
                fill: 'red',
                strokeWidth: 1,
                radius: 5,
                name: name,
                draggable: true,
                dragOnTop: false,
                strokeScaleEnabled: false,
                anchor: true
            });

            anchor.on('dragmove', function () {
                updateShapePosition(this);
                layer.draw();
            });

            anchor.on('mouseover', function () {
                document.body.style.cursor = 'pointer';
                layer.draw();
            });

            anchor.on('mouseout', function () {
                document.body.style.cursor = 'default';
                layer.draw();
            });

            group.add(anchor);
        };
        
        var layer = group.getLayer();
        //var rect = group.get('Rect');
        
       // if (util.isEmpty(rect)) {
            
            var rect = new Kinetic.Rect({
                x: points[0].x,
                y: points[0].y,
                width: points[1].x - points[0].x,
                height: points[2].y - points[1].y,
                fill: '#fff',
                opacity: 0.3,
                stroke: '#fff',
                strokeWidth: 2,
                strokeScaleEnabled: false,
                draggable: true
            });
            
            rect.on('dragmove', function () {
                updateShapePosition(this);
                layer.draw();
            });
        
            group.add(rect);

            var rectDim = rect.getDimension();

            createAnchor(rectDim.left, rectDim.top, 'topLeft');
            createAnchor(rectDim.right, rectDim.top, 'topRight');
            createAnchor(rectDim.right, rectDim.bottom, 'bottomRight');
            createAnchor(rectDim.left, rectDim.bottom, 'bottomLeft');

            rect.moveToBottom();


        layer.draw();

        var updateShapePosition = function(node) {
            var grup = node.getParent();
            var topLeft = grup.get('.topLeft')[0];
            var topRight = grup.get('.topRight')[0];
            var bottomRight = grup.get('.bottomRight')[0];
            var bottomLeft = grup.get('.bottomLeft')[0];
            var rekt = grup.get('Rect')[0];

            if (node.className == 'Rect') {

                var rektDim = rekt.getDimension();
                
                //var rekt = Rect(imgPos.x, imgPos.y, imgSize.width, imgSize.height);

                topLeft.setPosition(rektDim.left, rektDim.top);
                topRight.setPosition(rektDim.right, rektDim.top);
                bottomRight.setPosition(rektDim.right, rektDim.bottom);
                bottomLeft.setPosition(rektDim.left, rektDim.bottom);

            } else {

                if (node.className == 'Circle') {
                    var anchorX = node.getX();
                    var anchorY = node.getY();

                    // update anchor positions
                    switch (node.getName()) {
                        case 'topLeft':
                            topRight.setY(anchorY);
                            bottomLeft.setX(anchorX);
                            break;
                        case 'topRight':
                            topLeft.setY(anchorY);
                            bottomRight.setX(anchorX);
                            break;
                        case 'bottomRight':
                            bottomLeft.setY(anchorY);
                            topRight.setX(anchorX);
                            break;
                        case 'bottomLeft':
                            bottomRight.setY(anchorY);
                            topLeft.setX(anchorX);
                            break;
                    }
                }

                rekt.setPosition(topLeft.getPosition());

                var width = topRight.getX() - topLeft.getX();
                var height = bottomLeft.getY() - topLeft.getY();
                if (width && height) {
                    rekt.setSize(width, height);
                }
            }
        };
    };
    
    var setGlassImagePosition = function (glassImage, data) {
        var roiWidth = main.glassTraceRect.getSize().width;
        //var faceHeight = main.faceAnchors[1].getAttr('y') - main.faceAnchors[3].getAttr('y');
        //var x = main.faceAnchors[0].getAttr('x') - (data.leftDistance);
        //var y = main.faceAnchors[3].getAttr('y') - (data.topDistance);
        var width = Math.round(parseFloat((data.widthPercentage / 100) * roiWidth) * 100) / 100;
        var height = Math.round(parseFloat((data.heightPercentage / 100) * roiWidth) * 100) / 100;
        var x = (Math.round(parseFloat((data.leftPercentage / 100) * roiWidth) * 100) / 100) + main.glassTraceRect.getAttr('x');
        var y = (Math.round(parseFloat((data.topPercentage / 100) * roiWidth) * 100) / 100) + main.glassTraceRect.getAttr('y');

        glassImage.setPosition(x, y);
        glassImage.setSize(width, height);
    };
    
    var tryOnGlass = function (imageSrc, data, group, callback) {
        //var photoPos = main.photo.getPosition();

        initNewItemImage(imageSrc, 'glass', group, function (image) {
            setGlassImagePosition(image, data);
            group.getLayer().draw();
            
            image.fire('dragmove');

            util.notify(callback, image);
        });
    };
    
    var getGlassModel = function () {

        var glassSize = main.glass.getSize();
        var glassPos = main.glass.getPosition();
        //var photoPos = main.photo.getPosition();
        var roiWidth = main.glassTraceRect.getSize().width; //main.faceAnchors[2].getAttr('x') - main.faceAnchors[0].getAttr('x');
        //var faceHeight = main.faceAnchors[1].getAttr('y') - main.faceAnchors[3].getAttr('y');
        var widthPercentage = Math.round(((glassSize.width / roiWidth) * 100) * 100) / 100;
        var heightPercentage = Math.round(((glassSize.height / roiWidth) * 100) * 100) / 100;
        var x = glassPos.x - main.glassTraceRect.getAttr('x');
        var y = glassPos.y - main.glassTraceRect.getAttr('y');
        var leftPercentage = Math.round(((x / roiWidth) * 100) * 100) / 100;
        var topPercentage = Math.round(((y / roiWidth) * 100) * 100) / 100;

        var data = {
            'LeftPercentage': leftPercentage,
            'TopPercentage': topPercentage,
            'WidthPercentage': widthPercentage,
            'HeightPercentage': heightPercentage
        };

        return data;
    };
    
    var tryOnHairstyle = function(imageSrc, data, group, callback) {
        //var photoPos = main.photo.getPosition();
        
        //var faceWidth = main.faceAnchors[2].getAttr('x') - main.faceAnchors[0].getAttr('x');
        ////var faceHeight = main.faceAnchors[1].getAttr('y') - main.faceAnchors[3].getAttr('y');
        ////var x = main.faceAnchors[0].getAttr('x') - (data.leftDistance);
        ////var y = main.faceAnchors[3].getAttr('y') - (data.topDistance);
        //var width = Math.round(parseFloat((data.widthPercentage / 100) * faceWidth) * 100) / 100;
        //var height = Math.round(parseFloat((data.heightPercentage / 100) * faceWidth) * 100) / 100;
        //var x = (Math.round(parseFloat((data.leftDistance / 100) * faceWidth) * 100) / 100) + main.faceAnchors[0].getAttr('x');
        //var y = (Math.round(parseFloat((data.topDistance / 100) * faceWidth) * 100) / 100) + main.faceAnchors[3].getAttr('y');

        initNewItemImage(imageSrc, 'hair', group, function (image) {
            //image.setPosition(x, y);
            //image.setSize(width, height);
            image.data = data;
            setHairPosition(image);
            group.getLayer().draw();

            //image.fire('dragmove');
            
            util.notify(callback, image);
        });
    };

    var setHairPosition = function (hair, faceAnchors) {

        if (!util.isEmpty(hair.data)) {
            faceAnchors = util.isEmpty(faceAnchors) ? main.faceAnchors : faceAnchors;
            //var faceWidth = main.faceAnchors[2].getAttr('x') - main.faceAnchors[0].getAttr('x');
            //var faceHeight = main.faceAnchors[1].getAttr('y') - main.faceAnchors[3].getAttr('y');
            var data = hair.data;
            ////var x = main.faceAnchors[0].getAttr('x') - (data.leftDistance);
            ////var y = main.faceAnchors[3].getAttr('y') - (data.topDistance);
            //var faceX = main.faceAnchors[0].getAttr('x');
            //var faceY = main.faceAnchors[3].getAttr('y');
            //var width = Math.round(parseFloat((data.widthPercentage / 100) * faceWidth) * 100) / 100;
            //var height = Math.round(parseFloat((data.heightPercentage / 100) * faceHeight) * 100) / 100;
            //var x = (Math.round(parseFloat((data.leftDistance / 100) * faceWidth) * 100) / 100) + faceX;
            //var y = (Math.round(parseFloat((data.topDistance / 100) * faceHeight) * 100) / 100) + faceY;

            var faceWidth = faceAnchors[2].getAttr('x') - faceAnchors[0].getAttr('x');
            var faceHeight = faceAnchors[1].getAttr('y') - faceAnchors[3].getAttr('y');
            var width = Math.round(parseFloat((data.widthPercentage / 100) * faceWidth) * 100) / 100;
            var height = Math.round(parseFloat((data.heightPercentage / 100) * faceHeight) * 100) / 100;
            var x = (Math.round(parseFloat((data.leftDistance / 100) * faceWidth) * 100) / 100) + faceAnchors[0].getAttr('x');
            var y = (Math.round(parseFloat((data.topDistance / 100) * faceHeight) * 100) / 100) + faceAnchors[3].getAttr('y');
            
            hair.setPosition(x, y);
            hair.setSize(width, height);

        }
    };
    
    var getHairstyleModel = function() {

        var hairSize = main.hair.getSize();
        var hairPos = main.hair.getPosition();
        //var photoPos = main.photo.getPosition();
        var faceWidth = main.faceAnchors[2].getAttr('x') - main.faceAnchors[0].getAttr('x');
        var faceHeight = main.faceAnchors[1].getAttr('y') - main.faceAnchors[3].getAttr('y');
        var widthPercentage = Math.round(((hairSize.width / faceWidth) * 100) * 100) / 100;
        var heightPercentage = Math.round(((hairSize.height / faceHeight) * 100) * 100) / 100;
        var x = hairPos.x - main.faceAnchors[0].getAttr('x');
        var y = hairPos.y - main.faceAnchors[3].getAttr('y');
        var leftDistance = Math.round(((x / faceWidth) * 100) * 100) / 100;
        var topDistance = Math.round(((y / faceHeight) * 100) * 100) / 100;

        var data = {
            'LeftDistance': leftDistance,
            'TopDistance': topDistance,
            'WidthPercentage': widthPercentage,
            'HeightPercentage': heightPercentage
        };

        return data;
    };

    var drawCurve = function (context, points) {
        
        context.beginPath();
        context.moveTo(points[0].x, points[0].y);

        var ap = points;
        var len = ap.length;
        var n = 0;
        
        while (n < len - 1) {
            context.bezierCurveTo(ap[n].x, ap[n++].y, ap[n].x, ap[n++].y, ap[n].x, ap[n++].y);
        }
        
        context.closePath();
    };

    var updateEyeClip = function(points, group) {
        //var blob = group.get('Blob');
        //var points = blob.allPoints;
        
        group.setClipFunc(function(deg) {
            drawCurve(deg.context, points);
        });
    };

    var addEyesTraceChangeEvents = function () {
        
        //var lBlob = main.stage.get('#traceleftEye')[0];
        //var rBlob = main.stage.get('#tracerightEye')[0];
        //var lAnchors = lBlob.getParent().get('Circle');
        //var rAnchors = rBlob.getParent().get('Circle');
        //var eyesLayer = main.stage.get('#eyesLayer')[0];
        //var leftEyeballGroup = eyesLayer.get('#leftEyeballGroup')[0];
        //var rightEyeballGroup = eyesLayer.get('#rightEyeballGroup')[0];
        //var lContact = leftEyeballGroup.get('Image')[0];
        //var rContact = rightEyeballGroup.get('Image')[0];
        
        //for (var i = 0; i < lAnchors.length; i++) {
        //    if (util.findByPropValue(lAnchors[i].eventListeners.dragend, 'name', 'clip').length <= 0
        //        /*$.grep(lAnchors[i].eventListeners.dragend, function(e){ return e.name === 'clip'; })*/) {
        //        lAnchors[i].on('dragend.clip', function() {
        //            updateEyeClip(lBlob.allPoints, leftEyeballGroup);
        //            eyesLayer.draw();
        //        });
        //    }
        //}
        
        //for (var i = 0; i < rAnchors.length; i++) {
        //    if (util.findByPropValue(rAnchors[i].eventListeners.dragend, 'name', 'clip').length <= 0) {
        //        rAnchors[i].on('dragend.clip', function () {
        //            updateEyeClip(rBlob.allPoints, rightEyeballGroup);
        //            eyesLayer.draw();
        //        });
        //    }
        //}
        
        //if(!util.isEmpty(lContact)) {
        //    if (util.findByPropValue(main.leftEyeballAnchor.eventListeners.dragend, 'name', 'clip').length <= 0) {
        //        main.leftEyeballAnchor.on('dragend.clip', function () {
        //            setContactPosition(main.leftEyeballCircle, lContact);
        //            eyesLayer.draw();
        //        });
        //    }
        //}
        
        //if (!util.isEmpty(rContact)) {
        //    if (util.findByPropValue(main.rightEyeballAnchor.eventListeners.dragend, 'name', 'clip').length <= 0) {
        //        main.rightEyeballAnchor.on('dragend.clip', function () {
        //            setContactPosition(main.rightEyeballCircle, rContact);
        //            eyesLayer.draw();
        //        });
        //    }
        //}
    };

    var removeEventFromElements = function(elements, eventName) {
        for(var i=0;i<elements.length;i++) {
            elements[i].off(eventName);
        }
    };

    var pointsClipFunc = function(canvas, points) {
        var context = canvas.getContext();
        drawCurve(context, points);
    };
    
    var createContacts = function(imgObj, layer, callback) {

        var leftEyePoints = main.stage.get('#traceleftEye')[0].allPoints;
        var rightEyePoints = main.stage.get('#tracerightEye')[0].allPoints;
        var leftEyeballGroup = null;
        var rightEyeballGroup = null;

        leftEyeballGroup = new Kinetic.Group({ clipFunc: pointsClipFunc('leftEyeballGroup', leftEyePoints) });
        rightEyeballGroup = new Kinetic.Group({ clipFunc: pointsClipFunc('rightEyeballGroup', rightEyePoints) });

        layer.add(leftEyeballGroup);
        layer.add(rightEyeballGroup);

        var lContact = new Kinetic.Image({
            id: 'leftContact',
            originalImage: imgObj,
            //opacity: 0.6,
        });

        var rContact = new Kinetic.Image({
            id: 'rightContact',
            originalImage: imgObj,
            //opacity: 0.6
        });

        blendContact(imgObj, 'left', function(imgL) {
            lContact.setImage(imgL);

            blendContact(imgObj, 'right', function(imgR) {
                rContact.setImage(imgR);

                var contacts = { left: lContact, right: rContact };
                util.notify(callback, contacts);
            });
        });

        setContactPosition(main.leftEyeballCircle, lContact);
        leftEyeballGroup.add(lContact);

        setContactPosition(main.rightEyeballCircle, rContact);
        rightEyeballGroup.add(rContact);
    };
    
    var tryOnContactLenses = function (opts) {
        
        $.ajax({
            url: opts.url,
            tyep: 'POST',
            datatype: 'json',
            beforeSend: !util.isEmpty(opts.beforeSend) ? opts.beforeSend : undefined,
            //complete: !util.isEmpty(opts.complete) ? opts.complete : undefined,
            error: function (error) { alert(error); },
            success: function (response) {
                
                if (response.Success) {
                    var newLayer = false;
                    var imgObj = new Image();
                    var data = $.parseJSON(response.Data.data);

                    imgObj.onload = function() {

                        var layerId = 'eyesLayer';
                        var layer = main.stage.get('#' + layerId)[0];

                        if (util.isEmpty(layer)) {
                            newLayer = true;
                            layer = new Kinetic.Layer({ id: layerId, makeoverLayer: true });
                            main.stage.add(layer);
                            layer.setZIndex(1);
                        } else {
                            layer.removeChildren();
                        }

                        var eyeballsGroup = new Kinetic.Group({ id: 'eyeballsGroup' });
                        layer.add(eyeballsGroup);

                        var lContact = new Kinetic.Image({
                            id: 'leftContact',
                            originalImage: imgObj,
                            fileName: data.fileName
                        });

                        var rContact = new Kinetic.Image({
                            id: 'rightContact',
                            originalImage: imgObj,
                            fileName: data.fileName
                        });

                        blendContact(imgObj, 'left', function(imgL) {
                            lContact.setImage(imgL);
                            blendContact(imgObj, 'right', function(imgR) {
                                rContact.setImage(imgR);

                                var contacts = { left: lContact, right: rContact };
                                //main.contacts = contacts;
                                //main.contacts.fileName = response.fileName; //set the current contact file name
                                
                                layer.draw();

                                if (!util.isEmpty(opts.complete)) {
                                    util.notify(opts.complete); //call comlete callback
                                }

                                util.notify(opts.callback, contacts);
                            });
                        });

                        setContactPosition(main.leftEyeballCircle, lContact);
                        //leftEyeballGroup.add(lContact);
                        eyeballsGroup.add(lContact);

                        setContactPosition(main.rightEyeballCircle, rContact);
                        //rightEyeballGroup.add(rContact);
                        eyeballsGroup.add(rContact);
                    };
                    
                    imgObj.src = response.Data.imageSrc;

                    if (newLayer) {
                        main.stage.draw();
                    }
                }
            }
        });
    };
    
    var blendContact = function (eyeballImg, side, callback) {
        
        var eyeballCircle = null, eyeBlob = null;

        switch (side) {
            case 'left':
                eyeballCircle = main.stage.get('#traceleftEyeball')[0]; //main.leftEyeballCircle;
                eyeBlob = main.stage.get('#traceleftEye')[0];
                break;
            case 'right':
                eyeballCircle = main.stage.get('#tracerightEyeball')[0]; //main.rightEyeballCircle;
                eyeBlob = main.stage.get('#tracerightEye')[0];
                break;
        }

        var canvas = document.createElement('canvas');
        var srcImg = main.photo.getImage();
        var dstImg = eyeballImg;
        var context = canvas.getContext("2d");
        var photoPos = main.photo.getPosition();
        var maxWidth = srcImg.width;
        var maxHeight = srcImg.height;
        var width = Math.floor(eyeballCircle.getWidth());
        var height = Math.floor(eyeballCircle.getHeight());
        var points = removePhotoPosFromCoords(eyeBlob.allPoints);
        var x = ((eyeballCircle.getPosition().x - (width / 2)) - photoPos.x);
        var y = ((eyeballCircle.getPosition().y - (height / 2)) - photoPos.y);

        canvas.width = maxWidth;
        canvas.height = maxHeight;
        //context.fillRect(0, 0, maxWidth, maxHeight);

        context.save();
        drawCurve(context, points);
        context.clip();

        context.beginPath();
        context.arc(x + (width / 2), y + (height / 2), eyeballCircle.getRadius(), 0, 2 * Math.PI, false);
        context.closePath();
        context.clip();

        var pixels = 4 * width * height;

        context.drawImage(srcImg, x, y, width, height, x, y, width, height);
        var image1 = context.getImageData(x, y, width, height);
        var imageData1 = image1.data;
        context.drawImage(dstImg, x, y, width, height);
        var image2 = context.getImageData(x, y, width, height);
        var imageData2 = image2.data;

        //while (pixels--) {
        //    imageData1[pixels] = imageData1[pixels] * 0.5 + imageData2[pixels] * 0.5;
        //}

        var amount = 0.7;
        var amount2 = amount;
        var amount1 = 1 - amount;
        var p = pixels;

        while (p--) {
            var pixx = p * 4;
            var r = (imageData1[pixx] * amount1 + imageData2[pixx] * amount2) >> 0;
            var g = (imageData1[pixx + 1] * amount1 + imageData2[pixx + 1] * amount2) >> 0;
            var b = (imageData1[pixx + 2] * amount1 + imageData2[pixx + 2] * amount2) >> 0;

            imageData1[pixx] = r;
            imageData1[pixx + 1] = g;
            imageData1[pixx + 2] = b;
        }

        image1.data = imageData1;
        context.putImageData(image1, x, y);

        var canvas2 = document.createElement('canvas');
        canvas2.width = width;
        canvas2.height = height;
        var ctx = canvas2.getContext('2d');

        ctx.drawImage(canvas, x, y, width, height, 0, 0, width, height);

        var img = new Image();
        img.src = canvas2.toDataURL("image/png");
        img.onload = function () {

            util.notify(callback, img);
        };
    };
    
    var setContactPosition = function (traceEyeball, eyeballImage) {

        var w = traceEyeball.getSize().width;
        var h = traceEyeball.getSize().height;
        var x = traceEyeball.getPosition().x - (w / 2);
        var y = traceEyeball.getPosition().y - (h / 2);

        eyeballImage.setPosition(x, y);
        eyeballImage.setSize(w, h);
    };

    var convertAnchorsToCoordsData = function (anchors) {
        var photoPos = main.photo.getPosition();
        var coords = [];
                
        for (var i = 0; i < anchors.length; i++) {
            coords.push({
                "x": Math.floor(anchors[i].attrs.x - photoPos.x),
                "y": Math.floor(anchors[i].attrs.y - photoPos.y)
            });
        }
        var obj = { 'coords': coords };

        return obj;
    };
    
    var convertEyeballToCoordsData = function (anchor, removePhotoPos) {

        var anchr = util.clone(anchor);
        var photoPos = main.photo.getPosition();
        
        if (removePhotoPos) {
            anchr.setAttr('x', anchor.getAttr('x') - photoPos.x);
            anchr.setAttr('y', anchor.getAttr('y') - photoPos.y);
        }

        var obj = {
            'pupilCoord': { x: Math.floor(anchr.attrs.x), y: Math.floor(anchr.attrs.y) },
            'radius': Math.floor(anchr.attrs.radius)
        };

        return obj;
    };
    
    var initAnchors = function (points, shapeId, group) {

        switch (shapeId) {
            case faceId:
                main.faceAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.faceAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }

                break;
            case leftEyeId:
                main.leftEyeAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.leftEyeAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }
                break;
            case rightEyeId:
                main.rightEyeAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.rightEyeAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }
                break;
            case leftEyeBrowId:
                main.leftEyeBrowAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.leftEyeBrowAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }
                break;
            case rightEyeBrowId:
                main.rightEyeBrowAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.rightEyeBrowAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }
                break;
            case mouthId:
                main.mouthAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.mouthAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }
                break;
            case openLipsId:
                main.openLipsAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.openLipsAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }
                break;
            case glassId:
                main.glassTraceAnchors = []; //Clear all existing anchors for this feature

                for (var i = 0; i < points.length; i++) {
                    main.glassTraceAnchors.push(buildAnchor(points[i].x, points[i].y, shapeId, group));
                }
                break;
            default:
        }
    };

    var redrawAnchors = function (shapeId, container) {
        //var clonedAnchors = [];

        switch (shapeId) {
            case faceId:
                var count = main.faceAnchors.length;
                
                for (var i = 0; i < count;i++) {
                    container.add(main.faceAnchors[i]);
                }
                //clonedAnchors = util.clone(main.faceAnchors); //copy all existing anchors to this variable

                //main.faceAnchors = []; //Clear all existing anchors for this feature

                //for (var i = 0; i < clonedAnchors.length; i++) {
                //    main.faceAnchors.push(buildAnchor(clonedAnchors[i].attrs.x, clonedAnchors[i].attrs.y, shapeId, container));
                //}
                break;
            case leftEyeId:
                var count = main.leftEyeAnchors.length;

                for (var i = 0; i < count; i++) {
                    container.add(main.leftEyeAnchors[i]);
                }
                
                //clonedAnchors = util.clone(main.leftEyeAnchors); //copy all existing anchors to this variable

                //main.leftEyeAnchors = []; //Clear all existing anchors for this feature

                //for (var i = 0; i < clonedAnchors.length; i++) {
                //    main.leftEyeAnchors.push(buildAnchor(clonedAnchors[i].attrs.x, clonedAnchors[i].attrs.y, shapeId, container));
                //}
                break;
            case rightEyeId:
                var count = main.rightEyeAnchors.length;

                for (var i = 0; i < count; i++) {
                    container.add(main.rightEyeAnchors[i]);
                }
                
                //clonedAnchors = util.clone(main.rightEyeAnchors); //copy all existing anchors to this variable

                //main.rightEyeAnchors = []; //Clear all existing anchors for this feature

                //for (var i = 0; i < clonedAnchors.length; i++) {
                //    main.rightEyeAnchors.push(buildAnchor(clonedAnchors[i].attrs.x, clonedAnchors[i].attrs.y, shapeId, container));
                //}
                break;
            case leftEyeBrowId:
                var count = main.leftEyeBrowAnchors.length;

                for (var i = 0; i < count; i++) {
                    container.add(main.leftEyeBrowAnchors[i]);
                }
                
                //clonedAnchors = util.clone(main.leftEyeBrowAnchors); //copy all existing anchors to this variable

                //main.leftEyeBrowAnchors = []; //Clear all existing anchors for this feature

                //for (var i = 0; i < clonedAnchors.length; i++) {
                //    main.leftEyeBrowAnchors.push(buildAnchor(clonedAnchors[i].attrs.x, clonedAnchors[i].attrs.y, shapeId, container));
                //}
                break;
            case rightEyeBrowId:
                var count = main.rightEyeBrowAnchors.length;

                for (var i = 0; i < count; i++) {
                    container.add(main.rightEyeBrowAnchors[i]);
                }
                
                //clonedAnchors = util.clone(main.rightEyeBrowAnchors); //copy all existing anchors to this variable

                //main.rightEyeBrowAnchors = []; //Clear all existing anchors for this feature

                //for (var i = 0; i < clonedAnchors.length; i++) {
                //    main.rightEyeBrowAnchors.push(buildAnchor(clonedAnchors[i].attrs.x, clonedAnchors[i].attrs.y, shapeId, container));
                //}
                break;
            case mouthId:
                var count = main.mouthAnchors.length;

                for (var i = 0; i < count; i++) {
                    container.add(main.mouthAnchors[i]);
                }
                
                //clonedAnchors = util.clone(main.mouthAnchors); //copy all existing anchors to this variable

                //main.mouthAnchors = []; //Clear all existing anchors for this feature

                //for (var i = 0; i < clonedAnchors.length; i++) {
                //    main.mouthAnchors.push(buildAnchor(clonedAnchors[i].attrs.x, clonedAnchors[i].attrs.y, shapeId, container));
                //}
                break;
            case openLipsId:
                var count = main.openLipsAnchors.length;

                for (var i = 0; i < count; i++) {
                    container.add(main.openLipsAnchors[i]);
                }
                
                //clonedAnchors = util.clone(main.openLipsAnchors); //copy all existing anchors to this variable

                //main.openLipsAnchors = []; //Clear all existing anchors for this feature

                //for (var i = 0; i < clonedAnchors.length; i++) {
                //    main.openLipsAnchors.push(buildAnchor(clonedAnchors[i].attrs.x, clonedAnchors[i].attrs.y, shapeId, container));
                //}
                break;
            case glassesId:
                var count = main.glassTraceAnchors.length;

                for (var i = 0; i < count; i++) {
                    container.add(main.glassTraceAnchors[i]);
                }

                break;
            default:
        }
    };
    
    var buildAnchor = function (x, y, shapeId, group) {
        var layer = group.getLayer();
        
        var anchor = new Kinetic.Circle({
            x: x,
            y: y,
            //radius: 3,
            stroke: '#00ff',
            fill: 'red',
            strokeWidth: 1,
            draggable: true,
            strokeScaleEnabled: false,
            id: 'trace' + shapeId + 'Anchor',
            name: 'anchor',
            anchor: true,
            anchorFor: shapeId //Custom property to hold the blob Id this anchor is associated with
        });

        // add hover styling
        anchor.on('mouseover', function () {

            document.body.style.cursor = 'pointer';
            this.setFill('transparent');

            lastActiveBlobAnchor = this.attrs.anchorFor;
            layer.draw();
        });

        anchor.on('mouseout', function () {

            document.body.style.cursor = 'default';
            this.setFill('red');
            lastActiveBlobAnchor = this.attrs.anchorFor;
            layer.draw();
        });
        
        //anchor.on('mousedown touchstart', function () {
        //    //main.stage.setDraggable(false);
        //    this.moveToTop();
        //});

        anchor.on('dragend', function() {
            lastActiveBlobAnchor = this.attrs.anchorFor;
            layer.draw();
        });

        group.add(anchor);

        return anchor;
    };

    var initShape = function (anchors, shapeId, tension, group) {
        var layer = group.getLayer();
        
        var points = util.anchorsToPoints(anchors);
        var shape = new Kinetic.Blob({
            points: points,
            stroke: '#fff',
            strokeWidth: 1,
            strokeScaleEnabled: false,
            fill: '#aaf',
            tension: tension,
            opacity: 0.2,
            id: 'trace' + shapeId,
            draggable: true
        });

        shape.on('mousemove', function () {
            lastActiveBlobAnchor = shapeId;
            document.body.style.cursor = 'pointer';
            layer.draw();
        });

        shape.on('mouseout', function () {
            lastActiveBlobAnchor = shapeId;
            document.body.style.cursor = 'default';
            layer.draw();
        });

        shape.was = { x: 0, y: 0 };

        group.add(shape);

        return shape;
    };

    var drawShape = function (shapeId) {

        //var shapeId = main.lastActiveBlobAnchor;
        var anchors = [];
        var group = null;
        var tension = 0.4;

        switch (shapeId) {
            case faceId:
                group = exec.Trace.faceGroup;
                anchors = main.faceAnchors;

                tension = 0.7;
                break;
            case leftEyeId:
                group = exec.Trace.leftEyeGroup;
                anchors = main.leftEyeAnchors;
                tension = 0.4234;// 0.3456;
                break;
            case rightEyeId:
                group = exec.Trace.rightEyeGroup;
                anchors = main.rightEyeAnchors;
                tension = 0.4234;// 0.3456;
                break;
            case leftEyeBrowId:
                group = exec.Trace.leftEyeBrowGroup;
                anchors = main.leftEyeBrowAnchors;
                tension = 0.3;
                break;
            case rightEyeBrowId:
                group = exec.Trace.rightEyeBrowGroup;
                anchors = main.rightEyeBrowAnchors;
                tension = 0.3;
                break;
            case mouthId:
                group = exec.Trace.mouthGroup;
                anchors = main.mouthAnchors;
                tension = 0.3456;
                break;
            case openLipsId:
                group = exec.Trace.openLipsGroup;
                anchors = main.openLipsAnchors;
                tension = 0.3456;
                break;
            default:
                tension = 0.3;
                break;
        }

        var shape = group.get('#trace' + shapeId)[0];

        //remove the shape is exist
        if (shape != null) {
            var shapePoints = util.anchorsToPoints(anchors); //.concat();
            //var isBlob = (shape.getShapeType() == "Blob");

            if (!shape.isDragging()) {
                for (var i = 0; i < shapePoints.length; i++) {
                    shapePoints[i].x -= shape.was.x;
                    shapePoints[i].y -= shape.was.y;
                }
                shape.setPoints(shapePoints);

            } else {
                for (var i = 0; i < anchors.length; i++) {
                    anchors[i].setX(anchors[i].getPosition().x + (shape.getX() - shape.was.x));
                    anchors[i].setY(anchors[i].getPosition().y + (shape.getY() - shape.was.y));
                }

                shape.was.x = shape.getPosition().x;
                shape.was.y = shape.getPosition().y;
            }
        } else {
            shape = initShape(anchors, shapeId, tension, group);

            //if(shapeId == main.openLipsId) {
            //    makeShapeComposite(shape, "xor");
            //}

            shape.moveToBottom();
            group.getLayer().draw();
        }
    };

    var drawEyeball = function (activeEyeball) {
        
        var group = activeEyeball.getParent();
        var anchor = group.get('.anchor')[0];
        var rad = activeEyeball.getRadius();

        if (activeEyeball.isDragging()) {
            var ax = activeEyeball.getX() + rad * Math.cos(15 * Math.PI / 180);
            var ay = activeEyeball.getY() + rad * Math.sin(15 * Math.PI / 180);
            anchor.setX(ax);
            anchor.setY(ay);

            switch (activeEyeball.getAttr('id')) {
                case leftEyeballId:
                    main.leftEyeballCircle = activeEyeball;
                    main.leftEyeballAnchor = anchor;
                    break;
                case rightEyeballId:
                    main.rightEyeballCircle = activeEyeball;
                    main.rightEyeballAnchor = anchor;
                    break;
            }
        }
    };

    var initEyeball = function (x, y, radius, group, id) {
        var layer = group.getLayer();

        var eyeball = new Kinetic.Circle({
            x: x,
            y: y,
            radius: radius,
            name: 'eyeball',
            stroke: '#fff',
            fill: '#aaf',
            opacity: 0.3,
            strokeWidth: 1,
            id: 'trace'+id,
            draggable: true,
            strokeScaleEnabled: false,
        });

        eyeball.on('dragmove', function () {
            drawEyeball(this);
            layer.draw();
        });

        eyeball.on('dragend', function () {
            drawEyeball(this);
            layer.draw();
        });

        var anchor = new Kinetic.Circle({
            x: x + radius * Math.cos(15 * Math.PI / 180), // Math.floor(x + (radius / 2 + radius / 3)),
            y: y + radius * Math.sin(15 * Math.PI / 180),
            stroke: 'blue',
            fill: 'red',
            strokeWidth: 1,
            strokeScaleEnabled: false,
            radius: 3,
            name: 'anchor',
            id: id + 'Anchor',
            draggable: true,
            dragBoundFunc: function (pos) {
                var eb = group.get('.eyeball')[0];
                var ebX = eb.getPosition().x;

                var newX = (pos.x + 2) >= ebX ? pos.x + 2 : pos.x;
                return {
                    x: newX, //pos.x,
                    y: this.getAbsolutePosition().y
                };
            },
            dragOnTop: false,
            anchor: true
        });

        anchor.on('dragmove', function () {
            updateEyeball(this);
            layer.draw();
        });

        anchor.on('dragend', function () {
            //group.setDraggable(true);
            layer.draw();
        });

        // add hover styling
        anchor.on('mouseover', function () {
            //var layer = this.getLayer();
            document.body.style.cursor = 'pointer';
            this.setFill('transparent');
            layer.draw();
        });

        anchor.on('mouseout', function () {
            //var layer = this.getLayer();
            document.body.style.cursor = 'default';
            this.setFill('red');
            layer.draw();
        });

        group.add(eyeball);
        group.add(anchor);

        eyeball.moveToBottom();

        layer.draw();

        return { 'eyeball': eyeball, 'anchor': anchor };
    };

    var updateEyeball = function (activeAnchor) {
        var group = activeAnchor.getParent();
        var eyeball = group.get('.eyeball')[0];
        var size = activeAnchor.getX() - eyeball.getX();

        if (size) {
            eyeball.setRadius(size);
        }

        switch (eyeball.attrs.id) {
            case leftEyeballId:
               main.leftEyeballCircle = eyeball;
                main.leftEyeballAnchor = activeAnchor;
                break;
            case rightEyeballId:
                main.rightEyeballCircle = eyeball;
                main.rightEyeballAnchor = activeAnchor;
                break;
        }
    };

    //What this variable does is keep track of how many times the user clicks on the photo to manually add trace anchors
    var bindManualTraceEvent = function (clickCount, callback) {

        //Add click event to model for creating anchors
        main.photo.on('click', function (e) {
            var offset = main.stage.getOffset(); //Get the stage offset
            var mousePos = main.stage.getMousePosition(); //Get the current mouse position on the stage

            var anchor = new Kinetic.Circle({
                x: (mousePos.x + offset.x) / main.scale,
                y: (mousePos.y + offset.y) / main.scale,
                radius: 3,
                draggable: true,
                fill: 'red',
                stroke: 'blue',
                strokeScaleEnabled: false,
                strokeWidth: 1
            });

            anchor.on('mouseover', function (e) {
                document.body.style.cursor = 'pointer';
                this.setFill('transparent');
                this.getLayer().draw();
            });

            anchor.on('mouseout', function (e) {
                document.body.style.cursor = 'default';
                this.setFill('red');
                this.getLayer.draw();
            });

            traceAmountClicked++;

            //If amount clicked plus (+) 1 is equal allowed click amount, remove the click event from the photo to 
            //stop creating anchor on click
            if (traceAmountClicked == clickCount) {
                main.photo.off('click');

                traceAmountClicked = 0; //Reset amount clicked
            }

            if (typeof (callback) == "function") {
                callback(anchor);
            }
        });
    };

    var clearTracePoints = function() {
        //Clear all global variables that will be set on this function
        main.facePoints = [];
        main.leftEyePoints = [];
        main.rightEyePoints = [];
        main.leftEyeBrowPoints = [];
        main.rightEyeBrowPoints = [];
        main.leftEyeballAnchors = [];
        main.rightEyeballPoints = [];
        main.mouthPoints = [];
        main.openLipsPoints = [];
        main.nosePoints = [];
        main.glassPoints = [];
    };
    
    var clearTraceAnchors = function() {

        //Clear all anchors
        main.faceAnchors = [];
        main.mouthAnchors = [];
        main.leftEyeAnchors = [];
        main.leftEyeballAnchor = null;
        main.leftEyeballCircle = null;
        main.leftEyeBrowAnchors = [];
        main.rightEyeAnchors = [];
        main.rightEyeBrowAnchors = [];
        main.rightEyeballAnchor = null;
        main.rightEyeballCircle = null;
        //main.leftEyeballAnchors = [];
        //main.rightEyeballAnchors = [];
        main.mouthAnchors = [];
        main.openLipsAnchors = [];
        main.noseAnchors = [];
        main.glassTraceAnchors = [];
        main.glassTraceRect = null;
    };
    
    var removePhotoPosFromCoords = function (coords) {
        var points = util.clone(coords);
        var photoPos = main.photo.getPosition();

        for (var i = 0; i < points.length; i++) {
            points[i].x = (points[i].x - photoPos.x);
            points[i].y = (points[i].y - photoPos.y);
        }

        return points;
    };
    
    var removePhotoPosFromAnchors = function (anchors) {
        var anchrs = util.clone(anchors);
        var photoPos = main.photo.getPosition();
        
        for (var i = 0; i < anchrs.length; i++) {
            anchrs[i].setAttr('x', anchrs[i].getAttr('x') - photoPos.x);
            anchrs[i].setAttr('y', anchrs[i].getAttr('y') - photoPos.y);
        }

        return anchrs;
    };
    
    var removePhotoPosFromAnchor = function (anchor) {

        var result = util.clone(anchor);
        var photoPos = main.photo.getPosition();

        result.setPosition(anchor.getAttr('x') - photoPos.x, anchor.getAttr('y') - photoPos.y);

        return result;
    };
    
    var removePhotoPosFromCircle = function (circle) {
        var cir = util.clone(circle);
        var photoPos = main.photo.getPosition();

        cir.setAttr('x', circle[i].getAttr('x') - photoPos.x);
        cir.setAttr('y', circle[i].getAttr('y') - photoPos.y);

        return cir;
    };
    
    var addPhotoPositionToCoords = function(coords) {

        var points = util.clone(coords);
        var photoPos = main.photo.getPosition();

        for (var i = 0; i < points.length; i++) {
            points[i].x = points[i].x + photoPos.x;
            points[i].y = points[i].y + photoPos.y;
        }

        return points;
    };

    var addPhotoPosToAnchors = function (anchors, photo) {
        var anchrs = util.clone(anchors);
        photo = util.isEmpty(photo) ? main.photo : photo;
        var photoPos = photo.getPosition();
        
        for (var i = 0; i < anchrs.length; i++) {
            anchrs[i].setAttr('x', anchrs[i].getAttr('x') + photoPos.x);
            anchrs[i].setAttr('y', anchrs[i].getAttr('y') + photoPos.y);
        }

        return anchrs;
    };
    
    var hideAnchors = function (anchors) {
        
        if(!util.isEmpty(anchors)) {
            
            for(var i =0; i < anchors.length; i++) {
                anchors[i].hide();
            }

        }
    };
    
    var processScaleFactor = function(stage)
    {
        stage = util.isEmpty(stage) ? main.stage : stage;
        stage.setPosition(0, 0);
        stage.setScale(1);
        
        stage.origin = {
            x: stage.getPosition().x + stage.getWidth() / 2,
            y: stage.getPosition().y + stage.getHeight() / 2
        };
        
        //stage.setScale(1);
        var scale = 1;
        var imgSize = main.photo.getSize();
        
        if (imgSize.height > imgSize.width) {
            scale = (stage.getHeight() / imgSize.height);
        } else {
            scale = (stage.getWidth() / imgSize.width);
        }

        //scale = scale > 3.5 ? 3.5 : scale;
        stage.scaleFactor = (scale);
        stage.scale = scale;
        stage.maxScale = Math.round(scale * 5);
    };

    var createHairContainersIfNotExist = function() {

        var layerId = 'hairLayer';
        var groupId = 'hairGroup';
        var newLayer = false;
        var layer = main.stage.get('#' + layerId)[0];
        var group = null;

        if (!util.isEmpty(layer)) {
            group = layer.get('#' + groupId)[0];

            if (!util.isEmpty(group)) {
                //destroyChildren(group);
                group.removeChildren();
            } else {
                group = new Kinetic.Layer({ id: groupId });
                layer.add(group);
            }
        } else {
            newLayer = true;
            layer = new Kinetic.Layer({ id: layerId, makeoverLayer: true });
            group = new Kinetic.Group({ id: groupId });
            layer.add(group);
            main.stage.add(layer);
            layer.setZIndex(90);
        }

        //if (!util.isEmpty(layer)) {
        //    layer.remove();
        //}

        //newLayer = true;
        //layer = new Kinetic.Layer({ id: layerId, makeoverLayer: true });
        //group = new Kinetic.Group({ id: groupId });
        //layer.add(group);
        //main.stage.add(layer);
        //layer.setZIndex(90);

        return { newLayer: newLayer, layer: layer, group: group };
    };

    var createGlassContainersIfNotExist = function() {
        var layerId = 'glassLayer';
        var groupId = 'glassGroup';
        var newLayer = false;
        var layer = main.stage.get('#' + layerId)[0];
        var group = null;

        if (!util.isEmpty(layer)) {
            group = layer.get('#' + groupId)[0];

            if (!util.isEmpty(group)) {
                //destroyChildren(group);
                group.removeChildren();
            } else {
                group = new Kinetic.Layer({ id: groupId });
                layer.add(group);
            }
        } else {
            newLayer = true;
            layer = new Kinetic.Layer({ id: layerId, makeoverLayer: true });
            group = new Kinetic.Group({ id: groupId });
            layer.add(group);
            main.stage.add(layer);
            layer.setZIndex(2);
        }
        
        return { newLayer: newLayer, layer: layer, group: group };
    };

    var clipHair = function () {
        var dim = main.photo.getDimension();
        main.hair.getLayer().setClip([dim.x, dim.y, dim.width, dim.height]);
        
        //main.hair.getLayer().setClip(function (deg) {
        //    var dim = main.photo.getDimension();
        //    var context = deg.context;
        //    context.beginPath();
        //    context.rect(dim.x, dim.y, dim.width, dim.height);
        //});
    };

    var getHair = function() {
        return main.stage.get('#hair')[0];
    };
    
    var getGlass = function () {
        return main.stage.get('#glass')[0];
    };
    
    var getContacts = function () {
        //var groups = main.stage.get('#eyeballsGroup')[0];
        var leftContact = main.stage.get('#leftContact')[0];
        var rightContact = main.stage.get('#rightContact')[0];
        
        if (util.isEmpty(leftContact) || util.isEmpty(rightContact)) {
            return null;
        }

        return { left: leftContact, right: rightContact };
    };
    
    this.init = function() {
        var processPhoto = function(photoSrc, callback) {
            var imageObj = new Image();

            imageObj.onload = function() {

                var x = (main.stage.getWidth() - imageObj.width) / 2;
                var y = (main.stage.getHeight() - imageObj.height) / 2;

                main.photo = new Kinetic.Image({
                    x: x,
                    y: y,
                    image: imageObj,
                    id: 'photo'
                });

                //main.photoX = main.photo.getPosition().x;
                //main.photoY = main.photo.getPosition().y;

                processScaleFactor();
                main.photoLayer.add(main.photo);
                main.photoLayer.draw();

                util.notify(callback);
            };

            imageObj.src = photoSrc;
        };

        var width = options.width || 300;
        var height = options.height || 300;

        this.stage = new Kinetic.Stage({
            container: options.container, x:0, y:0, width: width, height: height, offset: [0,0],
            draggable: options.draggable
        });

        this.stage.origin = {
            x: this.stage.getPosition().x + this.stage.getWidth() / 2,
            y: this.stage.getPosition().y + this.stage.getHeight() / 2
        };
        
        this.photoLayer = new Kinetic.Layer({id: 'photoLayer'});
        this.stage.add(this.photoLayer);
        this.photoLayer.setZIndex(0);
        //this.photoLayer.moveToBottom();
        this.stage.draw();

        this.stage.on('dragend', function (e) {
            var scale = this.getScaleX();
            this.origin = {x: this.getPosition().x + this.getWidth() / 2 * scale,
                y: this.cy = this.getPosition().y + this.getHeight() / 2 * scale};
        });

        if (!util.isEmpty(options.photoSrc)) {            
            processPhoto(options.photoSrc, options.callback);
        }else {
            util.notify(options.callback);
        }
    };

    var exec = {
        global: main,
        
        initPhoto: function(opts) {
            $.ajax({
                url: opts.url,
                dataType: 'json',
                type: 'POST',
                error: function (error) {
                    util.notify(opts.complete);
                    alert(error);
                },
                beforeSend: !util.isEmpty(opts.beforeSend) ? opts.beforeSend : undefined,
                //complete: !util.isEmpty(opts.complete) ? opts.complete : undefined,
                success: function(response) {
                    
                    if (response.Success) {
                        var photoImage = new Image();

                        photoImage.onload = function () {

                            var x = (main.stage.getWidth() - photoImage.width) / 2;
                            var y = (main.stage.getHeight() - photoImage.height) / 2;

                            main.photo = new Kinetic.Image({
                                x: x,
                                y: y,
                                image: photoImage,
                                id: 'photo',
                                originalImage: photoImage
                            });

                            main.photo.fileName = response.fileName;

                            processScaleFactor();

                            var coordData = $.parseJSON(response.Data);
                            response.Data = coordData;

                            exec.Trace.assignCoords(coordData);
                            exec.Trace.traceAndHideAnchors();

                            main.photoLayer.add(main.photo);
                            //main.stage.add(main.photoLayer);
                            //main.photoLayer.moveToBottom();
                            main.photoLayer.draw();

                            var complete = function () {
                                if (!util.isEmpty(opts.complete)) {
                                    util.notify(opts.complete);
                                }
                                util.notify(opts.callback, response);
                            };

                            complete();
                        };

                        photoImage.src = response.imageSrc;
                    } else {
                        util.notify(opts.complete);
                    }
                }
            });
        },

        changePhoto: function(opts) {
            $.ajax({
                url: opts.url,
                dataType: 'json',
                type: 'POST',
                beforeSend: !util.isEmpty(opts.beforeSend) ? opts.beforeSend : undefined,
                //complete: !util.isEmpty(opts.complete) ? opts.complete : undefined,
                error: function (error) {
                    util.notify(opts.complete);
                    alert(error);
                },
                success: function(response) {

                    if (response.Success) {
                        var modelImage = new Image();

                        modelImage.onload = function() {

                            var x = (main.stage.getWidth() - modelImage.width) / 2;
                            var y = (main.stage.getHeight() - modelImage.height) / 2;

                            main.photo.setImage(modelImage);
                            main.photo.fileName = response.fileName;
                            main.photo.setAttr('originalImage', modelImage);
                            main.photo.setPosition(x, y);

                            processScaleFactor();
                            
                            var coordData = $.parseJSON(response.Data);
                            response.Data = coordData;

                            exec.Trace.assignCoords(coordData);
                            exec.Trace.traceAndHideAnchors();

                            main.photoLayer.draw();
                            
                            var complete = function () {
                                if (!util.isEmpty(opts.complete)) {
                                    util.notify(opts.complete);
                                }
                                util.notify(opts.callback, response);
                            };

                            complete();
                        };

                        modelImage.src = response.imageSrc;
                    }else {
                        util.notify(opts.complete);
                    }
                }
            });
        },
        
        setPhoto: function (imgSrc, coordData, callback) {
            var photoImage = new Image();

            photoImage.onload = function () {

                var x = (main.stage.getWidth() - photoImage.width) / 2;
                var y = (main.stage.getHeight() - photoImage.height) / 2;

                var photo = main.photo;
                var layer = main.photoLayer;
                
                if(util.isEmpty(photo)) {
                    photo = new Kinetic.Image({
                        x: x,
                        y: y,
                        image: photoImage,
                        id: 'photo',
                        originalImage: photoImage
                    });
                    
                    layer.add(photo);
                    main.photo = photo;
                }else {
                    photo.setImage(photoImage);
                    photo.setPosition(x, y);
                }
                
                photo.fileName = imgSrc.substring(imgSrc.lastIndexOf('/') + 1).replace('.jpg',''); //.replace(/^.*[\\\/]/, '');

                processScaleFactor();

                //var coordData = $.parseJSON(response.Data);
                //response.Data = coordData;

                exec.Trace.assignCoords(coordData);
                exec.Trace.traceAndHideAnchors();
                
                layer.draw();

                util.notify(callback);
            };

            photoImage.src = imgSrc;
        },
        
        zoomToFace: function() {
            var roi = util.anchorsToRect(main.faceAnchors);

            roi.width += 20;
            roi.x -= 10;
            roi.height += 20;
            roi.y -= 10;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },

        zoomToLeftEye: function() {
            var roi = util.anchorsToRect(main.leftEyeAnchors);

            roi.width += 40;
            roi.x -= 20;
            roi.height += 40;
            roi.y -= 20;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },

        zoomToRightEye: function() {
            var roi = util.anchorsToRect(main.rightEyeAnchors);

            roi.width += 40;
            roi.x -= 20;
            roi.height += 40;
            roi.y -= 20;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },

        zoomToEyes: function() {
            var anchors = util.clone(main.leftEyeAnchors);
            anchors.push.apply(anchors, util.clone(main.rightEyeAnchors));
            var roi = util.anchorsToRect(anchors);

            roi.width += 20;
            roi.x -= 10;
            roi.height += 40;
            roi.y -= 20;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },

        zoomToEyeBrows: function() {
            var anchors = util.clone(main.leftEyeBrowAnchors);
            anchors.push.apply(anchors, main.rightEyeBrowAnchors);
            var roi = util.anchorsToRect(anchors);

            roi.width += 20;
            roi.x -= 10;
            roi.height += 40;
            roi.y -= 20;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },

        zoomToMouth: function() {
            var roi = util.anchorsToRect(main.mouthAnchors);

            roi.width += 30;
            roi.x -= 15;
            roi.height += 30;
            roi.y -= 15;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },
        
        zoomToOpenLips: function () {
            var roi = [];
            
            if (!util.isEmpty(main.faceAnchors)) {
                roi = util.anchorsToRect(main.mouthAnchors);
            }else {
                roi = util.anchorsToRect(main.openLipsAnchors);
            }

            roi.width += 30;
            roi.x -= 15;
            roi.height += 30;
            roi.y -= 15;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },

        resetZoom: function() {
            var pos = main.photo.getPosition();
            var size = main.photo.getSize();

            var roi = Rect(pos.x, pos.y, size.width, size.height);

            roi.width += 100;
            roi.x -= 100;
            roi.height += 100;
            roi.y -= 100;

            main.stage.setDraggable(true);
            this.zoomToRoi(roi);
        },

        zoomToRoi: function(roi) {

            main.stage.setOffset(0, 0); //reset stage offset
            main.stage.setPosition(0, 0);
            //main.layer.setPosition(0, 0); //reset mainLayer's position incase it was dragged
            var scale = 1;
            
            main.stage.setScale(1); //reset stage scale

            var offsetX, offsetY;

            offsetX = roi.x;
            offsetY = roi.y;

            if (roi.height > roi.width) {
                scale = (main.stage.getHeight() / roi.height);
            } else {
                scale = (main.stage.getWidth() / roi.width);
            }

            scale = scale > main.stage.maxScale ? main.stage.maxScale : scale;

            main.stage.setOffset(offsetX, offsetY);
            main.stage.setScale(scale);
            //main.stage.setPosition(roi.x + roi.width / 2 * scale, roi.y - roi.height / 2 * scale);
            main.stage.draw();
        },
        
        zoom: function (scale, stage) {

            stage = util.isEmpty(stage) ? main.stage : stage;

            if (scale == stage.scaleFactor) {
                stage.setScale(1);
                stage.setPosition(0, 0);
                stage.setOffset(0, 0);
                stage.origin = {
                    x: stage.getPosition().x + stage.getWidth() / 2,
                    y: stage.getPosition().y + stage.getHeight() / 2
                };
            }

            stage.setScale(scale);
            stage.setPosition(
                stage.origin.x - stage.getWidth() / 2 * scale,
                stage.origin.y - stage.getHeight() / 2 * scale);

            stage.draw();
        },
        
        processScaleFactor: function(stage) {
            processScaleFactor(stage);
        },
        
        getHair: function () {
            getHair();
        },
        
        clearMakeover: function () {
            var layers = main.stage.get('Layer');
            
            for(var i=0; i<layers.length; i++) {
                if(layers[i].getAttr('makeoverLayer')) {
                    layers[i].destroyChildren();
                    layers[i].remove();
                }
            }

            main.stage.draw();
        },
        
        hideAllMakeover: function () {
            var layers = main.stage.get('Layer');

            for (var i = 0; i < layers.length; i++) {
                if (layers[i].getAttr('makeoverLayer')) {
                    layers[i].hide();
                }
            }
        },
        
        showAllMakeover: function () {
            var layers = main.stage.get('Layer');

            for (var i = 0; i < layers.length; i++) {
                if (layers[i].getAttr('makeoverLayer')) {
                    layers[i].show();
                }
            }
        },
        
        drawStage: function () {
            main.stage.draw();
        },
        
        increaseNodeHeight: function (node, amount, keepRatio) {
            var h = node.getHeight() + amount;

            if (keepRatio) {
                var w = node.getWidth() * h / node.getHeight();
                node.setSize(w, h);
            } else {
                node.setHeight(h);
            }
        },
    
        decreaseNodeHeight: function (node, amount, keepRatio) {
            var h = node.getHeight() - amount;

            if (keepRatio) {
                var w = node.getWidth() * h / node.getHeight();
                node.setSize(w, h);
            } else {
                node.setHeight(h);
            }
        },
    
        increaseNodeWidth: function (node, amount, keepRatio) {
            var w = node.getWidth() + amount;

            if (keepRatio) {
                var h = node.getHeight() * w / node.getWidth();
                node.setSize(w, h);
            } else {
                node.setWidth(w);
            }
            
        },

        decreaseNodeWidth: function (node, amount, keepRatio) {
            var w = node.getWidth() - amount;

            if (keepRatio) {
                var h = node.getHeight() * w / node.getWidth();
                node.setSize(w, h);
            } else {
                node.setWidth(w);
            }
        },
        
        toImage: function (callback) {
            var stage = main.stage;
            
            var positionStage = function(scale) {
                if (scale == stage.scaleFactor) {
                    stage.setScale(1);
                    stage.setPosition(0, 0);
                    stage.setOffset(0, 0);
                    stage.origin = {
                        x: stage.getPosition().x + stage.getWidth() / 2,
                        y: stage.getPosition().y + stage.getHeight() / 2
                    };
                }

                stage.setScale(scale);
                stage.setPosition(
                    stage.origin.x - stage.getWidth() / 2 * scale,
                    stage.origin.y - stage.getHeight() / 2 * scale);
            };

            var currentScale = main.stage.getScaleX();
            //var self = this;
            var photo = main.photo;
            var stagePos = stage.getPosition();
            
            positionStage(stage.scaleFactor);
            var w = photo.getWidth() * main.stage.scaleFactor;
            var h = photo.getHeight() * main.stage.scaleFactor;
            
            main.stage.toImage({
                x: (stage.getWidth() - w) / 2,
                y: (stage.getHeight() - h) / 2,
                width: w,
                height: h,
                mimeType: 'image/png',
                callback: function (img) {
                    positionStage(currentScale);
                    stage.setPosition(stagePos.x, stagePos.y);
                    
                    util.notify(callback, img);
                }
            });
        },
        
        toDataURL: function (callback) {
            var currentScale = main.stage.getScaleX();
            var self = this;
            //main.stage.setScale(main.stage.scaleFactor);
            this.zoom(main.stage.scaleFactor);

            main.stage.toDataURL({
                mimeType: 'image/png',
                callback: function (img) {
                    self.zoom(currentScale);

                    util.notify(callback, img);
                }
            });
        },
        
        Calibration: {
            
            newHair: function (imageSrc, fileName, callback) {
                var containerInfo = createHairContainersIfNotExist();

                initNewItemImage(imageSrc, 'hair', containerInfo.group, function (image) {
                    main.hair = image;
                    main.hair.saveMode = 'new';
                    main.hair.fileName = fileName;
                    
                    if (containerInfo.newLayer) {
                        //main.stage.setDraggable(true);
                        main.stage.draw();
                    }else {
                        containerInfo.layer.draw();
                    }

                    util.notify(callback);
                });
            },

            saveHair: function (url, callback) {
                //var photoSize = this.photo.getSize();
                //var photoPos = this.photo.getPosition();
                var data = getHairstyleModel();
                data.FileName = main.hair.fileName;
                
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: data,
                    dataType: 'json',
                    error: function (error) {
                        alert(error);
                    },
                    success: function (response) {
                        util.notify(callback, response);
                    }
                });
            },

            newGlass: function (imageSrc, callback) {

                //var newLayer = false;
                //var layerId = 'glassLayer';
                //var groupId = 'glassGroup';
                //var layer = main.stage.get('#' + layerId)[0];
                //var group = null;

                //if (!util.isEmpty(layer)) {
                //    group = layer.get('#' + groupId)[0];

                //    if (!util.isEmpty(group)) {
                //        //destroyChildren(group);
                //        group.removeChildren();
                //    } else {
                //        group = new Kinetic.Layer({ id: groupId });
                //        layer.add(group);
                //    }
                //} else {
                //    newLayer = true;
                //    layer = new Kinetic.Layer({ id: layerId, makeoverLayer: true });
                //    group = new Kinetic.Group({ id: groupId });
                //    layer.add(group);
                //    main.stage.add(layer);
                //    layer.setZIndex(2);
                //}
                var containersInfo = createGlassContainersIfNotExist();
                initNewItemImage(imageSrc, 'glass', containersInfo.group, function (image) {
                    main.glass = image;
                    
                    if (containersInfo.newLayer) {
                        //main.stage.setDraggable(true);
                        main.stage.draw();
                    }else {
                        containersInfo.layer.draw();
                    }

                    util.notify(callback);
                });
            },

            saveGlass: function (url, callback) {
                var data = getGlassModel();

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: data,
                    dataType: 'json',
                    error: function (error) {
                        alert(error);
                    },
                    success: function (response) {
                        util.notify(callback, response);
                    }
                });
            },
        },

        Makeover: {
            tryOnHairstyle: function (opts) {

                $.ajax({
                    url: opts.url,
                    dataType: 'json',
                    beforeSend: !util.isEmpty(opts.beforeSend) ? opts.beforeSend : undefined,
                    //complete: !util.isEmpty(opts.complete) ? opts.complete : undefined,
                    error: function (err) {
                        util.notify(opts.complete);
                        alert(err);
                    },
                    success: function (response) {
                        if (response.Success) {
                            var containerInfo = createHairContainersIfNotExist();

                            var data = $.parseJSON(response.Data);

                            tryOnHairstyle(response.imageSrc, data, containerInfo.group, function (image) {
                                main.hair = image;
                                main.hair.saveMode = 'tryOn';
                                main.hair.data = data;
                                main.hair.fileName = response.fileName;

                                clipHair();
                                if (containerInfo.newLayer) {
                                    main.stage.draw();
                                }

                                var complete = function() {
                                    util.notify(opts.complete);

                                    util.notify(opts.callback);
                                };

                                util.notify(complete);
                            });
                        }else {
                            util.notify(opts.complete, null);
                        }
                    }
                });
            },
            
            changeHairColor: function (opts) {
                if(util.isEmpty(getHair())) {
                    alert('Please try-on hairstyle before choosing hair color');
                    return;
                }
                
                $.ajax({
                    url: opts.url,
                    data: {fn: main.hair.fileName},
                    dataType: 'json',
                    beforeSend: !util.isEmpty(opts.beforeSend) ? opts.beforeSend : undefined,
                    
                    error: function (err) {
                        util.notify(opts.complete);
                        alert(err);
                    },
                    success: function (response) {
                        if (response.Success) {

                            var hairImg = new Image();
                            hairImg.src = response.imageSrc;
                            
                            hairImg.onload = function () {
                               
                                main.hair.setImage(hairImg);
                                main.hair.getLayer().draw();

                                var complete = function() {
                                    util.notify(opts.complete);

                                    util.notify(opts.callback);
                                };

                                util.notify(complete);
                            };
                        } else {
                            util.notify(opts.complete);
                        }
                    }
                });
            },
            
            removeHairColor: function () {
                var originalHair = main.hair.getAttr('originalImage');
                
                if(!util.isEmpty(originalHair)) {
                    main.hair.setImage(originalHair);
                    main.hair.color = null;
                    main.hair.getLayer().draw();
                }
            },

            removeHairstyle: function () {
                if (!util.isEmpty(main.hair)) {
                    var layer = main.hair.getLayer();
                    main.hair.destroy();
                    layer.draw();

                    main.hair = null;
                }
            },
            
            tryOnContactLenses: function (opts) {
                tryOnContactLenses(opts);
            },
            
            redrawContacts: function (callback) {
                var contacts = getContacts();
                
                if(!util.isEmpty(contacts)) {
                    blendContact(contacts.left.attrs.originalImage, 'left', function (imgL) {
                        contacts.left.setImage(imgL);

                        blendContact(contacts.right.attrs.originalImage, 'right', function (imgR) {
                            contacts.right.setImage(imgR);

                            setContactPosition(main.leftEyeballCircle, contacts.left);
                            setContactPosition(main.rightEyeballCircle, contacts.right);

                            contacts.left.getLayer().draw();
                            util.notify(callback);
                        });
                    });
                }else {
                    util.notify(callback);
                }
            },
            
            getContacts: function () {
                return getContacts();
            },
            
            removeContacts: function () {
                var contacts = getContacts();
                
                if (!util.isEmpty(contacts)) {
                    var layer = contacts.left.getLayer();
                    contacts.left.destroy();
                    contacts.right.destroy();
                    layer.draw();

                    //main.contacts = null;
                }
            },
            
            tryOnGlass: function (opts) {
                $.ajax({
                    url: opts.url,
                    dataType: 'json',
                    beforeSend: !util.isEmpty(opts.beforeSend) ? opts.beforeSend : undefined,
                    error: function (err) {
                        util.notify(opts.complete);
                        alert(err);
                    },
                    success: function (response) {
                        if (response.Success) {

                            var containersInfo = createGlassContainersIfNotExist();
                            var data = $.parseJSON(response.Data);

                            tryOnGlass(response.imageSrc, data, containersInfo.group, function (image) {
                                main.glass = image;
                                main.glass.data = data;
                                main.glass.setAttr('fileName', data.fileName);
                                
                                if (containersInfo.newLayer) {
                                    //main.stage.setDraggable(true);
                                    main.stage.draw();
                                }

                                var complete = function () {
                                    util.notify(opts.complete);

                                    util.notify(opts.callback);
                                };

                                util.notify(complete);
                            });
                        }else {
                            util.notify(opts.complete);
                        }
                    }
                });
            },
            
            removeGlass: function () {
                if (!util.isEmpty(main.glass)) {
                    var layer = main.glass.getLayer();
                    main.glass.destroy();
                    layer.draw();

                    main.glass = null;
                }
            }
        },
        
        Trace: {            
            groups: { manualTraceGroup: null },
            eyesManualTraceAnchors: [],
            mouthManualTraceAnchors: [],
            noseManualTraceAnchors: [],

            init: function() {
                this.layer = new Kinetic.Layer({id: 'traceLayer'});
                var _this = this;

                this.layer.on('beforeDraw', function() {
                    
                    if(!util.isEmpty(main.photo)) {

                        if (lastActiveBlobAnchor != null) {
                            drawShape(lastActiveBlobAnchor);
                        }

                        var anchors = _this.layer.get('Circle');
                        var scale = main.stage.getScaleX();
                        var photoSize = main.photo.getSize();
                        var ratio = 1;
                        
                        if(photoSize.width > photoSize.height) {
                            ratio = photoSize.width / scale;
                        }else {
                            ratio = photoSize.height / scale;
                        }
                        
                        for (var i = 0; i < anchors.length; i++) {
                            if (anchors[i].getAttr('anchor')) {
                                if (ratio <= 89) {
                                    anchors[i].setRadius(1);
                                } else if (ratio <= 90) {
                                    anchors[i].setRadius(2);
                                } else if (ratio <= 300) {
                                    anchors[i].setRadius(3);
                                } else if (ratio <= 800) {
                                    anchors[i].setRadius(3);
                                } else if (ratio >= 900) {
                                    anchors[i].setRadius(5);
                                }
                            }
                        }
                        
                        //console.log(main.photo.getSize().width / scale);
                    }
                });
                
                this.layer.on('mousemove', function () {
                    if (this.isDraggable()) {
                        document.body.style.cursor = 'pointer';
                        main.stage.draw();
                    }
                });

                this.layer.on('mouseout', function () {
                    document.body.style.cursor = 'default';
                    main.stage.draw();
                });
                
                main.stage.add(this.layer);
                main.stage.draw();
            },

            initManualTrace: function(clickCount, callback) {

                this.clearLayer(); //remove all trace elements on the stage
                this.groups.manualTraceGroup = new Kinetic.Group();
                this.layer.add(this.groups.manualTraceGroup);

                //Add click event to model for creating anchors
                main.photo.on('click', function(e) {
                    var offset = main.stage.getPosition(); //getOffset(); //Get the stage offset
                    var mousePos = main.stage.getMousePosition(); //Get the current mouse position on the stage
                    var scl = main.stage.getScaleX();
                    
                    var anchor = new Kinetic.Circle({
                        x: (mousePos.x - offset.x) / scl,
                        y: (mousePos.y - offset.y) / scl,
                        radius: 3,
                        draggable: true,
                        fill: 'red',
                        stroke: 'blue',
                        strokeScaleEnabled: false,
                        strokeWidth: 1
                    });

                    anchor.on('mouseover', function(e) {
                        document.body.style.cursor = 'pointer';
                        this.setFill('transparent');
                        this.getLayer().draw();
                    });

                    anchor.on('mouseout', function(e) {
                        document.body.style.cursor = 'default';
                        this.setFill('red');
                        this.getLayer().draw();
                    });

                    traceAmountClicked++;

                    //If amount clicked plus (+) 1 is equal allowed click amount, remove the click event from the photo to 
                    //stop creating anchor on click
                    if (traceAmountClicked == clickCount) {
                        main.photo.off('click');

                        traceAmountClicked = 0; //Reset amount clicked
                    }

                    if (typeof(callback) == "function") {
                        callback(anchor);
                    }
                });

                main.stage.setDraggable(false);
                main.stage.draw();
            },

            assignCoords: function(data, callback) {

                clearTracePoints();
                
                //Remove the trace layer if exist
                //this.clearTraceLayer();

                //face
                main.facePoints = util.clone(data.face.coords);

                //left eye
                main.leftEyePoints = util.clone(data.leftEye.coords);
                main.leftEyeballPoint = {
                    'x': data.leftEyeball.pupilCoord.x,
                    'y': data.leftEyeball.pupilCoord.y
                };
                main.leftEyeballRadius = data.leftEyeball.radius;
                main.leftEyeBrowPoints = util.clone(data.leftEyeBrow.coords);

                //right eye
                main.rightEyePoints = util.clone(data.rightEye.coords);
                main.rightEyeballPoint = {
                    'x': data.rightEyeball.pupilCoord.x,
                    'y': data.rightEyeball.pupilCoord.y
                };
                main.rightEyeballRadius = data.rightEyeball.radius;
                main.rightEyeBrowPoints = util.clone(data.rightEyeBrow.coords);

                //nose
                main.nosePoints = addPhotoPositionToCoords(data.nose.coords);

                //mouth
                main.mouthPoints = util.clone(data.lips.coords);

                main.openLipsPoints = util.clone(data.openLips.coords);
                main.openLips = data.hasOpenLips;
                
                if(!data.hasOpenLips) {
                    if(!util.isEmpty(this.openLipsGroup)) {
                        this.openLipsGroup.removeChildren();
                    }
                }
                
                if (!util.isEmpty(data.glass)) {
                    main.glassPoints = util.clone(data.glass.coords);
                }
                
                util.notify(callback);
            },
            
            convertAnchorsToCoordsData: function (anchors) {
                return convertAnchorsToCoordsData(anchors);
            },
            
            convertEyeballToCoordsData: function (anchors) {
               return convertEyeballToCoordsData(anchors, true);
            },
            
            hideAllTrace: function () {
                
                if (!util.isEmpty(this.layer)) {
                    var count = this.layer.get('Group').length;
                    
                    for (var i = 0; i < count; i++) {
                        this.layer.get('Group')[i].hide();
                    }
                }

                //this.glassesGroup.show();
                //this.leftEyeGroup.show();
                //this.rightEyeGroup.show();
                //this.faceGroup.show();
                
                //this.faceGroup.hide();
                //this.leftEyeGroup.hide();
                //this.leftEyeballGroup.hide();
                //this.leftEyeBrowGroup.hide();
                //this.rightEyeGroup.hide();
                //this.rightEyeballGroup.hide();
                //this.rightEyeBrowGroup.hide();
                //this.mouthGroup.hide();
            },
            
            traceAndHideAnchors: function () {
                
                clearTraceAnchors();
                //this.destroyAllGroups();
                
                //Remove the trace layer if exist
                //this.clearTraceLayer();

                //face
                this.traceFace();

                //left eye
                this.traceLeftEye();

                this.traceLeftEyeball();
                //hideAnchors([main.leftEyeballAnchor, main.leftEyeballCircle]);

                this.traceLeftEyeBrow();
                //hideAnchors(main.leftEyeBrowAnchors);

                //right eye
                this.traceRightEye();
                //hideAnchors(main.rightEyeAnchors);
                
                this.traceRightEyeball();
                //hideAnchors([main.rightEyeballAnchor, main.rightEyeballCircle]);

                this.traceRightEyeBrow();
                //hideAnchors(main.rightEyeBrowAnchors);

                //nose
                //main.nosePoints = util.clone(data.nose.coords);

                //mouth
                this.traceMouth();
                //hideAnchors(main.mouthAnchors);
                
                if (main.openLips) {
                    this.traceOpenLips();
                    //hideAnchors(main.openLipsAnchors);
                }

                this.traceGlass();
                
                this.hideAllTrace();
                this.draw();
                
                //util.notify(callback);
            },
            
            generateGlassTracePoints : function () {
                var points = [];
                var width = main.faceAnchors[2].getAttr('x') - main.faceAnchors[0].getAttr('x');
                main.glassPoints = [];

                points.push({ x: main.faceAnchors[0].getAttr('x'), y: main.leftEyeAnchors[0].getAttr('y') - (width/4)/2 });
                points.push({ x: main.faceAnchors[0].getAttr('x') + width, y: main.leftEyeAnchors[0].getAttr('y') -(width / 4) / 2 });
                points.push({ x: main.faceAnchors[0].getAttr('x') + width, y: main.leftEyeAnchors[0].getAttr('y') + (width / 4) / 2 });
                points.push({ x: main.faceAnchors[0].getAttr('x'), y: main.leftEyeAnchors[3].getAttr('y') + (width / 4) / 2 });

                main.glassPoints = removePhotoPosFromCoords(points);
            },
    
            //What this function does is collect all manually added anchors for face, and then calculate and create points for
            //face, eyes, nose and mouth. 
            processAllManualTraceAnchors: function (callback) {

                //clear all effected points
                main.facePoints = [];
                main.leftEyePoints = [];
                main.rightEyePoints = [];
                main.leftEyeballPoints = [];
                main.rightEyeballPoints = [];
                main.leftEyeBrowPoints = [];
                main.rightEyeBrowPoints = [];
                main.noseAnchors = [];
                main.mouthPoints = [];

                //Since nose has only one anchor to indentify the nose tip, the trace anchor for nose is directly
                //added to the main nose anchor variable.
                
                main.noseAnchors = this.noseManualTraceAnchors[0];
                //var eyesAnchors = removePhotoPosFromAnchors(this.eyesManualTraceAnchors);
                //var mouthAnchors = removePhotoPosFromAnchors(this.mouthManualTraceAnchors);
                
                //Get the eye anchor with the minimum x coordinate, which is the one at the left side
                var lEyeAnchor = this.eyesManualTraceAnchors[0];

                //Get the eye anchor with the maximum x coordinate, which is the one at the right side
                var rEyeAnchor = this.eyesManualTraceAnchors[1];

                //Get the total width occupied by both eye anchors
                var totalEyeRegionWidth = rEyeAnchor.attrs.x - lEyeAnchor.attrs.x;

                var eyeWidth = totalEyeRegionWidth / 3; //devide width by three to get the size of each eye width
                var eyeHeight = eyeWidth / 3; //devide eye width by 3 to get the height of each eye.

                //Create points for left eye
                //var lEyePoints = [];
                main.leftEyePoints.push({ 'x': lEyeAnchor.attrs.x, 'y': lEyeAnchor.attrs.y });
                main.leftEyePoints.push({ 'x': lEyeAnchor.attrs.x + eyeWidth / 2, 'y': lEyeAnchor.attrs.y - (eyeHeight / 2) });
                main.leftEyePoints.push({ 'x': lEyeAnchor.attrs.x + eyeWidth, 'y': lEyeAnchor.attrs.y });
                main.leftEyePoints.push({ 'x': lEyeAnchor.attrs.x + eyeWidth / 2, 'y': lEyeAnchor.attrs.y + (eyeHeight / 2) });

                //Create points for right eye
                //var rEyePoints = [];
                main.rightEyePoints.push({ 'x': rEyeAnchor.attrs.x - eyeWidth, 'y': rEyeAnchor.attrs.y });
                main.rightEyePoints.push({ 'x': rEyeAnchor.attrs.x - eyeWidth / 2, 'y': rEyeAnchor.attrs.y - (eyeHeight / 2) });
                main.rightEyePoints.push({ 'x': rEyeAnchor.attrs.x, 'y': rEyeAnchor.attrs.y });
                main.rightEyePoints.push({ 'x': rEyeAnchor.attrs.x - eyeWidth / 2, 'y': rEyeAnchor.attrs.y + (eyeHeight / 2) });

                //create points for mouth

                var lMouthAnchor = this.mouthManualTraceAnchors[0]; //Get the outter left anchor of the mouth
                var rMouthAnchor = this.mouthManualTraceAnchors[1]; //Get the outter left anchor of the mouth
                var mouthWidth = rMouthAnchor.attrs.x - lMouthAnchor.attrs.x;
                var mouthHeight = mouthWidth - mouthWidth / 3;

                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x, 'y': lMouthAnchor.attrs.y });
                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x + mouthWidth / 3, 'y': lMouthAnchor.attrs.y - mouthHeight / 3 });
                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x + mouthWidth / 2, 'y': lMouthAnchor.attrs.y - mouthHeight / 3 + (mouthHeight / 8) });
                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x + mouthWidth / 2 + (mouthWidth / 4), 'y': lMouthAnchor.attrs.y - mouthHeight / 3 });
                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x + mouthWidth, 'y': rMouthAnchor.attrs.y });

                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x + mouthWidth / 2 + (mouthWidth / 4), 'y': lMouthAnchor.attrs.y + mouthHeight / 3 });
                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x + mouthWidth / 2, 'y': lMouthAnchor.attrs.y + mouthHeight / 3 + 2 });
                main.mouthPoints.push({ 'x': lMouthAnchor.attrs.x + mouthWidth / 3, 'y': lMouthAnchor.attrs.y + mouthHeight / 3 });

                //create points for face

                //Get the height distance between outter left eye and mouth anchors
                var eyeMouthHDistance = Math.floor(lMouthAnchor.attrs.y - lEyeAnchor.attrs.y);

                //To get the outter left point of the face, the following formula is used
                // outter left eye anchor's x - eye width / 2. same formula is applied to get the outter right face point, but main
                //time using outter right eye anchor's x + eye width. Hope main makes sense main moment

                main.facePoints = [];

                //face left point
                main.facePoints.push({
                    'x': Math.floor(lEyeAnchor.attrs.x - eyeWidth / 2),
                    'y': Math.floor(lEyeAnchor.attrs.y)
                });

                //main.facePoints.push({
                //    'x': Math.floor(main.manualMouthTraceAnchors[0].attrs.x - mouthWidth / 2),
                //    'y': Math.floor(main.manualMouthTraceAnchors[0].attrs.y)
                //});

                //face bottom/jaw point
                main.facePoints.push({
                    'x': Math.floor(lMouthAnchor.attrs.x + mouthWidth / 2),
                    'y': Math.floor(lMouthAnchor.attrs.y + eyeMouthHDistance / 2)
                });

                //face right point
                //main.facePoints.push({
                //    'x': Math.floor(main.manualMouthTraceAnchors[1].attrs.x + mouthWidth/2),
                //    'y': Math.floor(main.manualMouthTraceAnchors[1].attrs.y)
                //});

                main.facePoints.push({
                    'x': Math.floor(rEyeAnchor.attrs.x + eyeWidth / 2),
                    'y': Math.floor(rEyeAnchor.attrs.y)
                });

                //face hairline point
                //To get the hairline point, i get the height/vertical distance between the left eye and mouth trace anchors, 
                //devid it by two. The resulting value is used to get the hairline y coordinate by adding subtracting it from 
                //left eye trace anchor. Just hope main makes sense at main moment.

                main.facePoints.push({
                    'x': Math.floor(lEyeAnchor.attrs.x + totalEyeRegionWidth / 2),
                    'y': Math.floor(lEyeAnchor.attrs.y - (eyeMouthHDistance + eyeMouthHDistance / 2))
                });

                //Remove photo coordinates from the assigned points
                main.facePoints = removePhotoPosFromCoords(main.facePoints);
                main.leftEyePoints = removePhotoPosFromCoords(main.leftEyePoints);
                main.rightEyePoints = removePhotoPosFromCoords(main.rightEyePoints);
                main.mouthPoints = removePhotoPosFromCoords(main.mouthPoints);
                
                util.notify(callback);
            },

            processOpenLipsFromMouthManualTrace: function () {
                main.openLipsPoints = [];

                var openLipsHeight = (main.mouthAnchors[6].attrs.y - main.mouthAnchors[2].attrs.y) / 3;
                var topY = main.mouthAnchors[2].attrs.y + (openLipsHeight / 2); //top x coordinate
                var bottomY = main.mouthAnchors[5].attrs.y - (openLipsHeight / 3); //bottom y coordinate

                main.openLipsPoints.push({ "x": main.mouthAnchors[0].attrs.x + 2, "y": main.mouthAnchors[0].attrs.y });
                main.openLipsPoints.push({ "x": main.mouthAnchors[1].attrs.x + 2, "y": topY });
                main.openLipsPoints.push({ "x": main.mouthAnchors[2].attrs.x + 2, "y": topY });
                main.openLipsPoints.push({ "x": main.mouthAnchors[3].attrs.x + 2, "y": topY });
                main.openLipsPoints.push({ "x": main.mouthAnchors[4].attrs.x + 2, "y": main.mouthAnchors[4].attrs.y });

                main.openLipsPoints.push({ "x": main.mouthAnchors[5].attrs.x, "y": bottomY });
                main.openLipsPoints.push({ "x": main.mouthAnchors[6].attrs.x, "y": bottomY });
                main.openLipsPoints.push({ "x": main.mouthAnchors[7].attrs.x, "y": bottomY });
                
                //Remove photo position from open lips coordinate
                main.openLipsPoints = removePhotoPosFromCoords(main.openLipsPoints);
            },

            createEyeballsPointsFromEyesAnchors: function () {

                main.leftEyeballPoints = [];
                main.rightEyeballPoints = [];

                var photoPos = main.photo.getPosition();
                var lEyeAnchor = main.leftEyeAnchors[0]; //Get the start anchor for left eye
                //Get the left eye width
                var lEyeWidth = main.leftEyeAnchors[2].attrs.x - main.leftEyeAnchors[0].attrs.x;

                var rEyeAnchor = main.rightEyeAnchors[0]; //Get the start anchor for left eye
                //Get the left eye width
                var rEyeWidth = main.rightEyeAnchors[2].attrs.x - main.rightEyeAnchors[0].attrs.x;

                //create points for left eyeball. Points[0] and points[2] are used to determin the size of the eyeball circle,
                //points[1] indicates the iris, which is also used as the center of the eyeball circle.
                //The assumption is that the eyeball is located at the center of the eye, and it's half the size of the eye.
                main.leftEyeballPoint = {
                    'x': lEyeAnchor.attrs.x + (lEyeWidth / 2) - photoPos.x,
                    'y': lEyeAnchor.attrs.y - photoPos.y
                };
                main.leftEyeballRadius = Math.floor(lEyeWidth / 4);

                //main.leftEyeballPoints.push({ 'x': lEyeAnchor.attrs.x + (lEyeWidth / 3), 'y': lEyeAnchor.attrs.y });
                //main.leftEyeballPoints.push({ 'x': lEyeAnchor.attrs.x + lEyeWidth - (lEyeWidth / 3), 'y': lEyeAnchor.attrs.y });

                main.rightEyeballPoint = {
                    'x': rEyeAnchor.attrs.x + (rEyeWidth / 2) - photoPos.x,
                    'y': rEyeAnchor.attrs.y - photoPos.y
                };
                main.rightEyeballRadius = Math.floor(rEyeWidth / 4);

                //main.rightEyeballPoints.push({ 'x': rEyeAnchor.attrs.x + (lEyeWidth / 3), 'y': rEyeAnchor.attrs.y });
                //main.rightEyeballPoints.push({ 'x': rEyeAnchor.attrs.x + rEyeWidth - (rEyeWidth / 3), 'y': rEyeAnchor.attrs.y });
                
                
            },

            createEyeBrowsPointsFromEyesAnchors: function () {

                main.leftEyeBrowPoints = [];
                main.rightEyeBrowPoints = [];

                //Get the total width of left eye
                var lEyeWidth = main.leftEyeAnchors[2].attrs.x - main.leftEyeAnchors[0].attrs.x;
                //Get height of left eye
                var lEyeHeight = main.leftEyeAnchors[3].attrs.y - main.leftEyeAnchors[1].attrs.y;
                var lAnchor = main.leftEyeAnchors[0]; //Get the outter left anchor of left eye

                //Get the total width of right eye
                var rEyeWidth = main.rightEyeAnchors[2].attrs.x - main.rightEyeAnchors[0].attrs.x;
                //Get height of right eye
                var rEyeHeight = main.rightEyeAnchors[3].attrs.y - main.rightEyeAnchors[1].attrs.y;
                var rAnchor = main.rightEyeAnchors[0]; //Get the left most anchor of the right eye

                var lEyeBrowX = lAnchor.attrs.x - lEyeWidth / 3;
                var lEyeBrowY = lAnchor.attrs.y - (lEyeHeight + lEyeHeight / 2);

                //Calculate and add coordinates for left eyebrows
                main.leftEyeBrowPoints.push({ 'x': lEyeBrowX, 'y': lEyeBrowY + lEyeHeight / 3 });
                main.leftEyeBrowPoints.push({ 'x': lEyeBrowX + lEyeWidth / 3, 'y': lEyeBrowY - lEyeHeight / 4 });
                main.leftEyeBrowPoints.push({ 'x': lEyeBrowX + (lEyeWidth / 3 * 2), 'y': lEyeBrowY - lEyeHeight / 4 });
                main.leftEyeBrowPoints.push({ 'x': lEyeBrowX + lEyeWidth, 'y': lEyeBrowY + lEyeHeight / 4 });

                main.leftEyeBrowPoints.push({ 'x': lEyeBrowX + (lEyeWidth / 3 * 2), 'y': lEyeBrowY + lEyeHeight / 4 });
                main.leftEyeBrowPoints.push({ 'x': lEyeBrowX + lEyeWidth / 3, 'y': lEyeBrowY + lEyeHeight / 4 });

                var rEyeBrowX = rAnchor.attrs.x - rEyeWidth / 3;
                var rEyeBrowY = rAnchor.attrs.y - (rEyeHeight + rEyeHeight / 2);

                //Calculate and add coordinates for right eyebrows
                main.rightEyeBrowPoints.push({ 'x': rEyeBrowX, 'y': rEyeBrowY + rEyeHeight / 4 });
                main.rightEyeBrowPoints.push({ 'x': rEyeBrowX + rEyeWidth / 3, 'y': rEyeBrowY - rEyeHeight / 4 });
                main.rightEyeBrowPoints.push({ 'x': rEyeBrowX + (rEyeWidth / 3 * 2), 'y': rEyeBrowY - rEyeHeight / 4 });
                main.rightEyeBrowPoints.push({ 'x': rEyeBrowX + (rEyeWidth + rEyeWidth / 2 * 2), 'y': rEyeBrowY + rEyeHeight / 3 });

                main.rightEyeBrowPoints.push({ 'x': rEyeBrowX + (rEyeWidth / 3 * 2), 'y': rEyeBrowY + rEyeHeight / 4 });
                main.rightEyeBrowPoints.push({ 'x': rEyeBrowX + rEyeWidth / 3, 'y': rEyeBrowY + rEyeHeight / 4 });
                
                //Remove photo position from coords
                main.leftEyeBrowPoints = removePhotoPosFromCoords(main.leftEyeBrowPoints);
                main.rightEyeBrowPoints = removePhotoPosFromCoords(main.rightEyeBrowPoints);
            },

            saveAllTraceAnchors: function (opts) {
                //url, callback
                var data = {};

                //face coords
                data[faceId] = (convertAnchorsToCoordsData(main.faceAnchors));

                //left eye coords
                data[leftEyeId] = (convertAnchorsToCoordsData(main.leftEyeAnchors));

                //left eyeball coords
                data[leftEyeballId] = (convertEyeballToCoordsData(main.leftEyeballCircle, true));

                //left eyebrows coords
                data[leftEyeBrowId] = (convertAnchorsToCoordsData(main.leftEyeBrowAnchors));

                //right eye coords
                data[rightEyeId] = (convertAnchorsToCoordsData(main.rightEyeAnchors));

                //right eyeball coords
                data[rightEyeballId] = (convertEyeballToCoordsData(main.rightEyeballCircle, true));

                //right eye brow coords
                data[rightEyeBrowId] = (convertAnchorsToCoordsData(main.rightEyeBrowAnchors));

                //nose coords
                data[noseId] = (convertAnchorsToCoordsData(main.noseAnchors));

                //lips coords
                data[mouthId] = (convertAnchorsToCoordsData(main.mouthAnchors));

                if (main.openLips) {
                    //open mouth coords
                    data[openLipsId] = (convertAnchorsToCoordsData(main.openLipsAnchors));
                }
                //data, url, callback
                opts.data = data;
                this.saveTraceData(opts);
            },

            saveFaceTrace: function (url, callback) {
                var data = {};
                //face coords
                data[faceId] = (convertAnchorsToCoordsData(main.faceAnchors));
                this.saveTraceData({ data: data, url: url, callback: callback });
            },
            
            saveEyesTrace: function (url, callback) {
                var data = {};
                
                data[leftEyeId] = (convertAnchorsToCoordsData(main.leftEyeAnchors));
                data[rightEyeId] = (convertAnchorsToCoordsData(main.rightEyeAnchors));
                
                //this.saveTraceData(data, url, callback);
                this.saveTraceData({ data: data, url: url, callback: callback });
            },
            
            saveEyeballsTrace: function (url, callback) {
                var data = {};

                //left eyeball coords
                data[leftEyeballId] = (convertEyeballToCoordsData(main.leftEyeballCircle, true));
                
                //right eyeball coords
                data[rightEyeballId] = (convertEyeballToCoordsData(main.rightEyeballCircle, true));

                //this.saveTraceData(data, url, callback);
                this.saveTraceData({ data: data, url: url, callback: callback });
            },
            
            saveGlassTrace: function (url, callback) {
                var data = {};

                data[glassId] = (convertAnchorsToCoordsData(main.glassTraceAnchors));

                //this.saveTraceData(data, url, callback);
                this.saveTraceData({ data: data, url: url, callback: callback });
            },
            
            saveTraceData: function (opts) {
                //data, url, callback
                opts.data.fileName = main.photo.fileName;
                
                $.ajax({
                    data: { data: JSON.stringify(opts.data), openLips: main.openLips },
                    url: opts.url,
                    type: 'POST',
                    beforeSend: !util.isEmpty(opts.beforeSend) ? util.notify(opts.beforeSend) : undefined,
                    error: !util.isEmpty(opts.error) ? util.notify(opts.error) : undefined,
                    success: function (response) {
                        response.coordData = opts.data;
                        util.notify(opts.callback, response);
                    }
                });
            },

            //initTrace function must be called before calling this function
            traceFace: function (reset /*bool; indicate if the face anchors should be reset*/) {

                if (!util.isEmpty(this.faceGroup)) {
                    this.faceGroup.remove();
                }
                
                this.faceGroup = new Kinetic.Group();
                this.layer.add(this.faceGroup);
                
                var id = faceId;
                lastActiveBlobAnchor = id;

                if (util.isEmpty(main.faceAnchors) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    var points = addPhotoPositionToCoords(main.facePoints);
                    initAnchors(points, id, this.faceGroup);
                } else {
                    redrawAnchors(id, this.faceGroup);
                }

                drawShape(id);
            },

            //initTrace function must be called before calling this function
            traceLeftEye: function (reset) {

                if (!util.isEmpty(this.leftEyeGroup)) {
                    this.leftEyeGroup.remove();
                }

                this.leftEyeGroup = new Kinetic.Group();
                this.layer.add(this.leftEyeGroup);

                var id = leftEyeId;
                lastActiveBlobAnchor = id;

                if (util.isEmpty(main.leftEyeAnchors) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    var points = addPhotoPositionToCoords(main.leftEyePoints);
                    initAnchors(points, id, this.leftEyeGroup);
                } else {
                    redrawAnchors(id, this.leftEyeGroup);
                }

                drawShape(id);
            },

            //initTrace function must be called before calling this function
            traceRightEye: function (reset) {

                if (!util.isEmpty(this.rightEyeGroup)) {
                    this.rightEyeGroup.remove();
                }
                
                this.rightEyeGroup = new Kinetic.Group();
                this.layer.add(this.rightEyeGroup);

                var id = rightEyeId;
                lastActiveBlobAnchor = id;

                if (util.isEmpty(main.rightEyeAnchors) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    var points = addPhotoPositionToCoords(main.rightEyePoints);
                    initAnchors(points, id, this.rightEyeGroup);
                } else {
                    redrawAnchors(id, this.rightEyeGroup);
                }

                drawShape(id);
            },

            traceLeftEyeball: function (reset) {
                
                if (!util.isEmpty(this.leftEyeballGroup)) {
                    this.leftEyeballGroup.remove();
                }
                
                this.leftEyeballGroup = new Kinetic.Group();
                this.layer.add(this.leftEyeballGroup);

                var id = leftEyeballId;
                var eyeballX = 0;
                var eyeballY = 0;
                var radius = 0;

                lastActiveBlobAnchor = null;

                if (util.isEmpty(main.leftEyeballCircle) || reset) {
                    var photoPos = main.photo.getPosition();

                    eyeballX = main.leftEyeballPoint.x + photoPos.x;
                    eyeballY = main.leftEyeballPoint.y + photoPos.y;
                    radius = main.leftEyeballRadius;

                    var result = initEyeball(eyeballX, eyeballY, radius, this.leftEyeballGroup, id);
                    main.leftEyeballCircle = result.eyeball;
                    main.leftEyeballAnchor = result.anchor;

                } else {
                    eyeballX = main.leftEyeballCircle.getPosition().x;
                    eyeballY = main.leftEyeballCircle.getPosition().y;
                    radius = main.leftEyeballCircle.getRadius(); //getPosition().x - this.leftEyeballCircle.getPosition().x;

                    initEyeball(eyeballX, eyeballY, radius, this.leftEyeballGroup, id);
                }
            },

            //initTrace function must be called before calling this function
            traceRightEyeball: function (reset) {

                if (!util.isEmpty(this.rightEyeballGroup)) {
                    this.rightEyeballGroup.remove();
                }
                
                this.rightEyeballGroup = new Kinetic.Group();
                this.layer.add(this.rightEyeballGroup);

                var id = rightEyeballId;
                var eyeballX = 0;
                var eyeballY = 0;
                var radius = 0;

                lastActiveBlobAnchor = null;

                if (util.isEmpty(main.rightEyeballCircle) || reset) {
                    var photoPos = main.photo.getPosition();

                    eyeballX = main.rightEyeballPoint.x + photoPos.x;
                    eyeballY = main.rightEyeballPoint.y + photoPos.y;
                    radius = main.rightEyeballRadius;

                    var result = initEyeball(eyeballX, eyeballY, radius, this.rightEyeballGroup, id);
                    main.rightEyeballCircle = result.eyeball;
                    main.rightEyeballAnchor = result.anchor;

                } else {
                    eyeballX = main.rightEyeballCircle.getPosition().x;
                    eyeballY = main.rightEyeballCircle.getPosition().y;
                    radius = main.rightEyeballCircle.getRadius(); //getPosition().x - this.rightEyeballCircle.getPosition().x;

                    initEyeball(eyeballX, eyeballY, radius, this.rightEyeballGroup, id);
                }
            },

            traceLeftEyeBrow: function (reset) {

                if (!util.isEmpty(this.leftEyeBrowGroup)) {
                    this.leftEyeBrowGroup.remove();
                }
                
                this.leftEyeBrowGroup = new Kinetic.Group();
                this.layer.add(this.leftEyeBrowGroup);

                var id = leftEyeBrowId;
                lastActiveBlobAnchor = id;

                if (util.isEmpty(main.leftEyeBrowAnchors) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    var points = addPhotoPositionToCoords(main.leftEyeBrowPoints);
                    initAnchors(points, id, this.leftEyeBrowGroup);
                } else {
                    redrawAnchors(id, this.leftEyeBrowGroup);
                }

                drawShape(id);
            },

            traceRightEyeBrow: function (reset) {

                if (!util.isEmpty(this.rightEyeBrowGroup)) {
                    this.rightEyeBrowGroup.remove();
                }
                
                this.rightEyeBrowGroup = new Kinetic.Group();
                this.layer.add(this.rightEyeBrowGroup);

                var id = rightEyeBrowId;
                lastActiveBlobAnchor = id;

                if (util.isEmpty(main.rightEyeBrowAnchors) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    var points = addPhotoPositionToCoords(main.rightEyeBrowPoints);
                    initAnchors(points, id, this.rightEyeBrowGroup);
                } else {
                    redrawAnchors(id, this.rightEyeBrowGroup);
                }

                drawShape(id);
            },
            
            traceGlass: function (reset) {

                if (!util.isEmpty(this.glassesGroup)) {
                    this.glassesGroup.remove();
                }

                this.glassGroup = new Kinetic.Group();
                this.layer.add(this.glassGroup);

                var id = glassId;
                lastActiveBlobAnchor = null;

                if (util.isEmpty(main.glassTraceRect) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    if(util.isEmpty(main.glassPoints)) {
                        this.generateGlassTracePoints();
                    }
                    
                    var points = addPhotoPositionToCoords(main.glassPoints);
                    
                    //initAnchors(points, id, this.glassesGroup);
                    createTraceRect('trace' + id, this.glassGroup, points);
                    main.glassTraceRect = this.glassGroup.get('Rect')[0];
                    main.glassTraceAnchors = [];
                    main.glassTraceAnchors = this.glassGroup.get('Circle');
                } else {
                    //redrawAnchors(id, this.glassesGroup);
                    this.glassGroup.add(main.glassTraceRect);
                    var anchors = main.glassTraceAnchors;

                    for (var i = 0; i < anchors.length; i++) {
                        this.glassGroup.add(anchors[i]);
                    }

                    this.layer.draw();
                }
            },

            //initTrace function must be called before calling this function
            traceMouth: function (reset) {

                var id = mouthId;

                if (!util.isEmpty(this.mouthGroup)) {
                    this.mouthGroup.remove();
                }

                this.mouthGroup = new Kinetic.Group();
                this.layer.add(this.mouthGroup);

                lastActiveBlobAnchor = id;

                if (util.isEmpty(main.mouthAnchors) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    var points = addPhotoPositionToCoords(main.mouthPoints);
                    initAnchors(points, id, this.mouthGroup);
                } else {
                    redrawAnchors(id, this.mouthGroup);
                }

                drawShape(id);
            },

            traceOpenLips: function (reset) {

                var id = openLipsId;

                if (!util.isEmpty(this.openLipsGroup)) {
                    this.openLipsGroup.remove();
                }
                
                this.openLipsGroup = new Kinetic.Group();
                this.layer.add(this.openLipsGroup);

                lastActiveBlobAnchor = id;
                
                if (util.isEmpty(main.openLipsAnchors) || reset) {
                    //Initialize blob anchor, then draw and update blob on stage
                    var points = addPhotoPositionToCoords(main.openLipsPoints);
                    initAnchors(points, id, this.openLipsGroup);
                } else {
                    redrawAnchors(id, this.openLipsGroup);
                }
                
                drawShape(id);
            },

            draw: function () {
                this.layer.draw();
            },

            //Get all points that will make up the face rect, and draw a rectangle around the face
            drawTraceRectOnFace: function () {

                var fp = util.clone(main.facePoints); //Get face points
                fp = addPhotoPositionToCoords(fp);

                var r = util.pointsToRect(fp); //Convert points to face bounding rect
                
                this.clearLayer();

                this.groups.faceGroup = new Kinetic.Group();
                this.layer.add(this.groups.faceGroup);

                var rect = new Kinetic.Rect({
                    x: r.x,
                    y: r.y,
                    width: r.width,
                    height: r.height,
                    fill: '#fff',
                    stroke: 'red',
                    strokeWidth: 2,
                    //offset: [r.width/2,r.height/2],
                    opacity: 0.3,
                    id: 'autoTraceFaceRect'
                });
                //rect.rotateDeg(rotDiff);
                var radians = util.getRotateRadians(fp[0], fp[2]);
                rect.rotate(radians);
                //rect.setPosition(-rect.x, -rect.y);

                this.groups.faceGroup.add(rect);
                this.layer.draw();
            },

            clearLayer: function () {
                try {
                    this.layer.removeChildren();
                    //this.layer.draw();
                } catch (err) {
                    console.log(err);}
            }
        }
    };

    this.init();
    return exec;
};

VirtualMakeover.TraceWizard = function (settings) {

    var steps = settings.ctrls;
    var count = steps.size();
    //settings.hideBtnIndexes = settings.hideBtnIndexes == null ? [] : settings.hideBtnIndexes;

    var initStep = settings.initStep;

    if (initStep == null || initStep == false) {
        initStep = 0;
    }

    var arrPos = [];

    steps.each(function (i) {

        $(this).attr("id", "step" + i);
        $(this).wrapInner('<div class="wizardContent" />');
        $(this).append("<p id='step" + i + "commands'></p>");

        if (i == 0) {
            createNextButton(i, $(this).attr("title"));
            $("#step" + i).hide();
        } else if (i == count - 1) {
            $("#step" + i).hide();
            createFinishButton(i);
            createPrevButton(i);
        } else {
            $("#step" + i).hide();
            createNextButton(i, $(this).attr("title"));
            if (i > initStep)
                createPrevButton(i);
        }
    });

    //To start with a particular step
    $("#step" + initStep).fadeIn(2500);

    function createNextButton(i, title) {

        var stepName = "step" + i;
        var nextBtn = $('<a />', {
            'id': stepName + "Next",
            'class': settings.nextButtonCss,
            text: settings.nextButtonTitle,
        });

        $("#" + stepName + "commands").append(nextBtn);
        $("#" + stepName + "Next").bind("click", function (e) {

            if (title == undefined || (title != undefined && settings.beforeNextByTitle(title))) {
                if (settings.beforeNext(i)) {

                    var nextStep = settings.onNext(i);

                    $("#" + stepName).hide();

                    if (nextStep == null || nextStep == false) {
                        $("#step" + (i + 1)).fadeIn(1500); //Default is next li will be displayed on Next Click
                    } else {
                        $("#step" + nextStep).fadeIn(1500); //Jump Wizard to go to particular index
                    }
                    arrPos.push(i);
                }
            }
        });
    }

    function createPrevButton(i) {

        //if (i+1 < count) {//Only create the back button if we're not on the last step

        var stepName = "step" + i;
        var prevBtn = $('<a />', { text: settings.previousButtonTitle, 'class': settings.previousButtonCss, 'id': stepName + "Prev" });

        $("#" + stepName + "commands").append(prevBtn);

        $("#" + stepName + "Prev").bind("click", function (e) {

            //if (settings.beforeBack == null || settings.beforeBack == undefined) {

            $("#" + stepName).hide();
            $("#step" + arrPos.pop()).show();

            settings.onBack(i);
            //} else if (settings.beforeBack(i)) {

            //    $("#" + stepName).hide();
            //    $("#step" + arrPos.pop()).show();
            //}
        });
        //}
    }

    function createFinishButton(i) {

        var stepName = "step" + i;
        var finishBtn = $('<a />', { text: settings.finishButtonTitle, 'class': settings.nextButtonCss, 'id': stepName + "Next" });

        $("#" + stepName + "commands").append(finishBtn);
        $("#" + stepName + "Next").bind("click", function (e) {
            //$("#" + stepName).hide();
            //$("#step" + (i + 1)).fadeIn(2500);
            //$("#step" + (i + 1)).slideDown(1500);
            //settings.onFinish(i);
        });

    }

    //steps.show();
};


//Calibration wizard
var traceSteps = {
    totalSteps: 0,
    index: 0,
    container: '',
    steps: [],
    stepClass: '',
    titles: [],
    features: [],

    ready: function (containerId, stepsClass) {

        traceSteps.container = $('#' + containerId);
        traceSteps.stepClass = stepsClass;

        traceSteps.steps = $('.' + traceSteps.stepClass, traceSteps.container);

        traceSteps.totalSteps = traceSteps.steps.length;

        traceSteps.steps.each(function (i) {
            traceSteps.titles.push($(traceSteps.steps[i]).attr('title'));
            traceSteps.features.push($(traceSteps.steps[i]).attr('data-trace'));
        });

        traceSteps.container.prepend($('<div />', { 'class': 'wizard-titles' }));
        var $titleDiv = $('.wizard-titles:first', traceSteps.container);

        for (var a = 0; a < traceSteps.titles.length; a++) {
            $titleDiv.append($('<span />', { 'class': 'title-step-' + a }).html(traceSteps.titles[a]));
        }

        traceSteps.updateUI();
        traceSteps.process(true);
    },

    next: function () {
        if (traceSteps.index == traceSteps.totalSteps - 1) {
            return false;
        }

        traceSteps.index++;
        traceSteps.updateUI();

        traceSteps.process();
    },

    back: function () {

        if (traceSteps.index <= 0) {
            return false;
        }

        traceSteps.index--;
        traceSteps.updateUI();

        traceSteps.process(false);
    },

    updateUI: function () {
        $('.wizard-titles span', traceSteps.container).removeClass('active');

        $('.wizard-titles span.title-step-' + traceSteps.index, traceSteps.container).addClass('active');

        traceSteps.steps.hide();
        $(traceSteps.steps[traceSteps.index]).show();
    },

    process: function (reset) {

        switch (traceSteps.features[traceSteps.index]) {
            case "Face":
                makeover.calibrateFace(reset);
                break;
            case "LeftEye":
                makeover.calibrateLeftEye(reset);
                break;
            case "RightEye":
                makeover.calibrateRightEye(reset);
                break;
            case "Mouth":
                makeover.calibrateMouth(reset);
                break;
        }
    }
};

//Initialize makeover before initializing this object
var automaticTraceComplete = {
    container: null,

    init: function (_container) {
        automaticTraceComplete.container = $(_container);
        automaticTraceComplete.container.show();

        makeover.drawTraceRectOnFace();
    },

    ok: function () {
        //save all points to database
    },
    bad: function () {
        //start manual tracing
    }
};

var Rect = function (x, y, width, height) {

    var rect = {
        x: x,
        y: y,
        width: width,
        height: height,
        top: y,
        bottom: y + height,
        right: x + width,
        left: x,
    };

    return rect;
};