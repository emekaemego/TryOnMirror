var _initStep = 0; //Initial wizard start step
var isLipsOpen = false; //Indicate if the user's mouth is opened in picture
//Indicate if the tracing is manual. That's if system fails to detect face or returned user indicates that
//returned face is wrong
var isManualTrace = false;
var traceClickCount = 5; //Number of manual trace anchors to be added
var traceClicked = 0; //keeps track of how time the image has been clicked to add manual trace anchor
var traceMakeover = null;
var traceWizardUI = null;
var wizardSettings = null;
var traceWizard = null;
var traceCoordsData = null;
var traceWizard = null;
//var vmNewPhotoTrace = null;
//var calibration = null;
var util = VirtualMakeover.Util;
var currentSection = 'photos';
var newHairSaveUrl = $('#saveNewHairUrl').attr('href');
var vm = null;
var vmNewPhotoTrace = null;
var glassTraceSaved = false;
var isRetracing = false;
var traceUpdateUrl = $('#updateTraceUrl').attr('href');
var prevItemStates = 'sections'; //{ sections: true, trace: false, adjuster: false };
var currItemStates = 'sections'; //{ sections: true, trace: false, adjuster: false };
var traceZoom = $('#traceZoomSlider');
var zoomSlider = $('#zoomSlider');
var isBefore = false;
var photoInit = false;
var traceImgSrcs = { };

$(document).ready(function() {
    var calibSaveBtn = $('#btnSaveCalibration');
    calibSaveBtn.attr('disabled', 'disabled');

    var photoContainer = $('#photo');
    vm = new VirtualMakeover.Main({
        container: 'photo',
        width: photoContainer.width(),
        height: photoContainer.height(),
        draggable: true,
        callback: function () {
            //$("#zoomSlider").slider();
        }
    });

    var hideRetracing = function() {
        if (isRetracing) {
            vm.Trace.hideAllTrace();
            isRetracing = false;
            //$('#virtualMakeover a.retrace-photo').removeClass('bold');
            
            if (isBefore) {
                afterMakeover();
            }else {
                vm.showAllMakeover();
            }
            vm.drawStage();
        }
    };

    var switchItemState = function(current) {

        prevItemStates = $('#components > div:visible')[0].id;

        if (current !== 'retrace') {
            hideRetracing();
        }

        if (current == 'adjuster') {
            $('#components input[name=keepRatio]').prop('checked', true);
        }

        $('#components > div').hide();
        $('#components #' + current).show();

        currItemStates = $('#components > div:visible')[0].id;
    };

    var showPrevItemState = function() {
        $('#components > div').hide();
        hideRetracing();
        $('#components #' + prevItemStates).show();
        prevItemStates = currItemStates;
    };

    var createSharePhoto = function(callback) {
        vm.toImage(function(img) {

            var maxImgWidth = (img.width > 400) ? 400 : img.width;
            var maxImgHeight = (img.height > 500) ? 500 * (500 / 400) : img.height;

            var ratio = 1;

            if (img.height > maxImgHeight) {
                ratio = maxImgHeight / img.height;
            } else if (img.width > maxImgWidth) {
                ratio = maxImgWidth / img.width;
            }

            var w = img.width * ratio;
            var h = img.height * ratio;

            var beforeImgW = 100 * ratio;
            var beforeImgH = 200 * ratio;
            var canvasW = (w + beforeImgW) + 50;
            var canvasH = (h + beforeImgH) + 150;
            var canvas = document.createElement('canvas');
            var ctx = canvas.getContext("2d");
            
            canvas.width = canvasW;
            canvas.height = canvasH;

            var imgLogo = new Image();
            imgLogo.src = $('#logo').attr('src');
            imgLogo.onload = function() {
                ctx.drawImage(imgLogo, 15, 15);

                var text = { font: 'italic 30px Times New Roman', label: 'Virtual Makeover', x: 15, y: imgLogo.height + 45 };
                ctx.save();
                ctx.font = text.font;
                ctx.fillStyle = '#ED3237';
                ctx.fillText(text.label, text.x, text.y);
                ctx.restore();

                ctx.save();
                ctx.beginPath();
                ctx.lineWidth = 2;
                ctx.moveTo(15, text.y + 10);
                ctx.lineTo(canvasW - 20, text.y + 10);
                ctx.closePath();
                ctx.stroke();
                ctx.restore();

                var imgCreate = new Image();
                imgCreate.src = $('#createBtn').attr('src');
                imgCreate.onload = function() {
                    ctx.drawImage(imgCreate, canvasW - (imgCreate.width + 20), text.y - 30);

                    var imgObj = new Image();
                    imgObj.src = img.src;
                    imgObj.onload = function() {
                        ctx.drawImage(imgObj, beforeImgW + 30, text.y + 30, w, h);
                        
                        var thumb = new Image();
                        thumb.src = vm.global.photo.getAttr('originalImage').src;
                        thumb.onload = function() {
                            ctx.drawImage(thumb, 15, text.y + 30, beforeImgW, beforeImgH);
                        
                            window.open(canvas.toDataURL());
                        };
                    };
                };
            };
            
        });
    };
    
    //vm.Calibration.init();
    vm.Trace.init();

    var processSlider = function() {
        var stage = vm.global.stage;
        var zoom = $('#zoomSlider');
        
        if ($('#zoomSlider:data(ui-slider)').length <= 0) {
            zoom.slider({
                min: stage.scaleFactor,
                value: stage.scale,
                max: stage.maxScale,
                animate: 'fast',
                step: stage.scaleFactor / 8,
                slide: function(event, ui) {
                    vm.zoom(ui.value);
                }
            });

        } else {
            zoom.slider("option", "value", stage.scaleFactor);
            zoom.slider("option", "min", stage.scaleFactor);
            zoom.slider("option", "max", stage.maxScale);
            zoom.slider("option", "step", stage.scaleFactor / 8);
        }
        //console.log($("#zoomSlider").slider("option", "step"));
        vm.zoom(stage.scaleFactor);
    };
    
    var processTraceZoomSlider = function () {
        var stage = vmNewPhotoTrace.global.stage;
        //var zoom = $('#traceZoomSlider');

        if ($('#traceZoomSlider:data(ui-slider)').length <= 0) {
            traceZoom.slider({
                min: stage.scaleFactor,
                value: stage.scale,
                max: stage.maxScale,
                animate: 'fast',
                step: stage.scaleFactor / 8,
                slide: function (event, ui) {
                    vmNewPhotoTrace.zoom(ui.value);
                }
            });
        }
        
        vmNewPhotoTrace.zoom(stage.scaleFactor);
    };

    var updateTraceZoomSlider = function() {
        if ($('#traceZoomSlider:data(ui-slider)').length > 0) {
            traceZoom.slider("option", "value", vmNewPhotoTrace.global.stage.getScale().x);
        }
    };

    var toggleTrace = function() {
        
        switch (currentSection.toLowerCase()) {
        case 'hair':
        case 'hairstyles':
        case 'haircolors':
            showFaceTrace();
            break;
        case 'eyes':
        case 'eyeglasses':
        case 'sunglasses':
            showGlassTrace();
            break;
        case 'contacts':
            showEyeballsTrace();
            break;
        }
    };

    var afterMakeover = function() {

        if (isBefore) {
            //var stage = vm.global.stage;

            //var layers = stage.get('Layer');
            //for (var i = 0; i < layers.length; i++) {
            //    layers[i].setVisible(true);
            //    layers[i].draw();
            //}
            vm.showAllMakeover();
            vm.global.stage.draw();
            isBefore = false;
        }
    };

    var beforeMakeover = function() {

        if (!isBefore) {
            //var stage = vm.global.stage;
            
            //var layers = stage.get('Layer');
            //for (var i = 0; i < layers.length; i++) {
            //    //dontHide = (!(layers[i].attrs.id == 'photoLayer') || !(layers[i].attrs.id == 'traceLayer'));
            //    //console.log(layers[i].attrs.id, dontHide);
            //    if (!(layers[i].attrs.id == 'photoLayer' || layers[i].attrs.id == 'traceLayer')) {
            //        layers[i].setVisible(false);
            //        layers[i].draw();
            //    }
            //}
            vm.hideAllMakeover();
            vm.global.stage.draw();
            isBefore = true;
        }
    };

    $('input[name=eyeTraceSwitch]').click(function(e) {
        var $this = $(this);

        vm.Trace.hideAllTrace();
        if ($this.val() == 'eyeballs') {
            showEyeballsTrace();
            $('#imgEyes').hide();
            $('#imgEyeballs').show();
        } else {
            showEyesTrace();
            $('#imgEyeballs').hide();
            $('#imgEyes').show();
        }
    });

    var saveCurrentTrace = function() {
        switch (currentSection.toLowerCase()) {
        case 'modephotos':
        case 'hair':
            case 'hairstyles':
            case 'haircolors':
            vm.Trace.saveFaceTrace(traceUpdateUrl);
            break;
        //case 'eyes':
        case 'contacts':
            var data = { };

            data['leftEye'] = (vm.Trace.convertAnchorsToCoordsData(vm.global.leftEyeAnchors));
            data['rightEye'] = (vm.Trace.convertAnchorsToCoordsData(vm.global.rightEyeAnchors));
            //left eyeball coords
            data['leftEyeball'] = (vm.Trace.convertEyeballToCoordsData(vm.global.leftEyeballCircle));
            //right eyeball coords
            data['rightEyeball'] = (vm.Trace.convertEyeballToCoordsData(vm.global.rightEyeballCircle));

            vm.Makeover.redrawContacts();
            vm.Trace.saveTraceData({data:data, url: traceUpdateUrl });
            break;
        case 'eyeglasses':
        case 'sunglasses':
            vm.Trace.saveGlassTrace(traceUpdateUrl);
            break;
        }
        
        showPrevItemState();
    };

    var retrace = function() {
        var trace = $('#virtualMakeover #components .retrace-wrapper');
        $('#eyeTraceSwitchEyeballs').prop('checked', true);
        
        vm.Trace.hideAllTrace();
        vm.hideAllMakeover();
        vm.drawStage();

        if (!trace.is(":visible")) {
            //section.hide('fast');
            isRetracing = true;
            $('#virtualMakeover a.retrace-photo').addClass('bold');
            //hideRetracing();
            $('#virtualMakeover #components .retrace-container li[class*="trace"]').hide();

            switch (currentSection.toLowerCase()) {
            case 'hair':
            //case 'photos':
                case 'hairstyles':
                case 'haircolors':
                showFaceTrace();
                $('#virtualMakeover #components .retrace-container li.face-trace').show();
                break;
            case 'eyes':
            case 'eyeglasses':
            case 'sunglasses':
                showGlassTrace();
                $('#virtualMakeover #components .retrace-container li.glasses-trace').show();
                break;
            case 'contacts':
                showEyeballsTrace();
                $('#virtualMakeover #components .retrace-container li.eyeballs-trace').show();
                break;
            }
            switchItemState('retrace');
            //$('#virtualMakeover #items .retrace-wrapper').show('slow');
        } else {
            isRetracing = true;
            showPrevItemState();
            //$('#virtualMakeover #items .retrace-wrapper').hide('fast');
            //section.show('slow');

            //if (isRetracing) {
            //    vm.Trace.hideAllTrace();
            //    isRetracing = false;
            //    //vm.Trace.draw();
            //    vm.showAllMakeover();
            //    vm.drawStage();
            //}
        }
    };

    var showFaceTrace = function() {
        var group = vm.global.faceAnchors[0].getParent();

        if (group.getVisible()) {
            group.hide();
        } else {
            group.show();
        }
        group.getLayer().draw();
    };

    var showGlassTrace = function() {
        var group = vm.global.glassTraceAnchors[0].getParent();

        if (group.getVisible()) {
            group.hide();
        } else {
            group.show();
        }
        group.getLayer().draw();
    };

    var showEyesTrace = function() {
        var lEyeGroup = vm.global.leftEyeAnchors[0].getParent();
        var rEyeGroup = vm.global.rightEyeAnchors[0].getParent();

        if (lEyeGroup.getVisible()) {
            lEyeGroup.hide();
            rEyeGroup.hide();
        } else {
            lEyeGroup.show();
            rEyeGroup.show();
        }

        lEyeGroup.getLayer().draw();
    };

    var showEyeballsTrace = function() {
        var lEyeballGroup = vm.global.leftEyeballCircle.getParent();
        var rEyeballGroup = vm.global.rightEyeballCircle.getParent();

        if (lEyeballGroup.getVisible()) {
            lEyeballGroup.hide();
            rEyeballGroup.hide();
        } else {
            lEyeballGroup.show();
            rEyeballGroup.show();
        }

        lEyeballGroup.getLayer().draw();
    };

    var processItemsDraggables = function() {
        var glass = vm.global.glass;
        var hair = vm.global.hair;
        
        var setHairstyleDraggable = function(drag) {
            if (!util.isEmpty(hair)) {
                if (drag === true) {
                    hair.setDraggable(true);
                    hair.clearImageHitRegion();
                } else {
                    hair.setDraggable(false);
                    hair.createImageHitRegion();
                }

                hair.getLayer().draw();
            }
        };
        
        var setGlassDraggable = function (drag) {
            if (!util.isEmpty(glass)) {
                var glassLayer = null;

                glass = vm.global.glass;
                glassLayer = glass.getLayer();
                
                if (drag === true) {
                    glass.setDraggable(true);
                } else {
                    glass.setDraggable(false);
                }

                glassLayer.draw();
            }
        };
        
        switch(currentSection) {
            case 'hair':
            case 'hairstyles':
            case 'hairColors':
                setHairstyleDraggable(true);
                setGlassDraggable(false);
                break;
            case 'eyes':
            case 'eyeglasses':
            case 'sunglasses':
                setHairstyleDraggable(false);
                setGlassDraggable(true);
                break;
            default:
                setHairstyleDraggable(false);
                setGlassDraggable(false);
                break;
        }
    };
    
    $('#virtualMakeover a.retrace-photo').click(function(e) {
        e.preventDefault();
        retrace();
    });

    $('#virtualMakeover ul.retrace-container li.controls a.save-trace').click(function(e) {
        e.preventDefault();
        saveCurrentTrace();
    });

    $('#virtualMakeover #components li.controls a.cancel, #retrace li.controls a.cancel').click(function (e) {
        e.preventDefault();
        //showItemsSections();
        showPrevItemState();
    });

    $('#virtualMakeover #lnkShareFacebbok').click(function(e) {
        e.preventDefault();
        //var shareWindow = window.open('about:blank', 'facebook-share-dialog');

        var $el = $(this);
        var loader = $('#busyModal');
        
        loader.modal();

        vm.toImage(function(img) {

            var imgData = img.src.replace('data:image/png;base64,', '');
            var provider = $el.attr('data-provider');
            //console.log(img);
            
            $.ajax({
                type: 'POST',
                url: $('#finalizemakeover').attr('href'),
                dataType: 'json',
                data: { ImageData: imgData, PhotoFileName: vm.global.photo.fileName, Provider: provider },
                error: function(errorThrown) {
                    loader.modal('hide');
                    alert(errorThrown);
                },
                complete: function() {
                    loader.modal('hide');
                },
                success: function(response) {
                    if (response.Success) {
                        if (provider == 'Facebook') {
                           var fbShare =  $('<a />', {
                                href: 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(response.url),
                                target: '_blank',
                                text: 'Click to share now', 'class': 'btn btn-facebook'
                           }).prepend('<i class="icon-facebook"></i>&nbsp;');
                            
                           $('#shareModal .modal-body').html(fbShare);
                            $('#shareModal').modal();

                            //url = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(response.url);
                            //window.open('https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(response.url), '_blank');
                        }
                    } else {
                        alert(response.Message);
                    }
                }
            });
        });
    });

    var resizeItem = function(element) {

        var id = element.id;
        var amount = 2;
        var node = null;
        var keepRatio = $('input[name=keepRatio]').prop('checked');

        var changeSize = function() {
            switch (id) {
            case 'plusWidth':
                vm.increaseNodeWidth(node, amount, keepRatio);
                break;
            case 'minusWidth':
                vm.decreaseNodeWidth(node, amount, keepRatio);
                break;
            case 'plusHeight':
                vm.increaseNodeHeight(node, amount, keepRatio);
                break;
            case 'minusHeight':
                vm.decreaseNodeHeight(node, amount, keepRatio);
                break;
            }
        };

        switch (currentSection.toLowerCase()) {
        case 'hair':
        case 'hairstyles':
            node = vm.global.hair;

            if (!util.isEmpty(node)) {
                changeSize();
                vm.global.hair.getLayer().draw();
            }
            break;
        case 'sunglasses':
        case 'eyeglasses':
            node = vm.global.glass;

            if (!util.isEmpty(node)) {
                changeSize();
                vm.global.glass.getLayer().draw();
            }
            break;
        }
    };
    
    var mainBusy = $('#mainPhotoWrapper .busy');

    $('#adjuster .description a').mousehold(150, function () {
        resizeItem(this);
    });

    $('#adjuster .description a').bind('click mouseheld', function(e) {
        e.preventDefault();
        resizeItem(this);
    });
    
    $('#components').on('click', '.adjust-control', function(e) {
        e.preventDefault();
        switchItemState('adjuster');
    });
    
    $('#virtualMakeover a.clear-makeover').click(function(e) {
        e.preventDefault();
        vm.clearMakeover();
    });
    
    $('#virtualMakeover .controls a.toggle-trace').click(function (e) {
        e.preventDefault();
        toggleTrace();
    });
    
    //'#photos, #eyes, #hair, #lips, #accessories, #hairstyles, #eyeglasses, #sunglasses'
    $('#virtualMakeover').on('click', '#makeoverMainMenu li a, #makeoverBody .sub-menu li a', function (e) {
        var $this = $(this);
        var photo = vm.global.photo;
        
        if(!photoInit && util.isEmpty(photo)) {
            initPhoto($('a#defaultModelPhoto').attr('href'));
        }
        
        var id = $this.attr('href').replace('#', '');

        if (id == 'photos') {
            $('#tryOnZone').addClass('hide'); //.hide();
        } else {
            $('#tryOnZone').removeClass('hide'); //.show();
        }
        
        var div = $('#' + id);
        currentSection = id;

        if (id != 'photos') {
            switchItemState('items');
        }

        if (id == 'eyes' || id == 'eyeglasses' || id == 'sunglasses') {
            glassesCoordsFirstTimeSave();
        }
        
        switch(id) {
            case 'hair':
            case 'eyes':
            case 'lips':
            case 'accessories':
                var firstDiv = $('div:visible:first', div);
                if(firstDiv.length > 0) {
                    currentSection = firstDiv.get(0).id;
                }
                
                if (id == 'eyeglasses' || id == 'sunglasses') {
                    glassesCoordsFirstTimeSave();
                }
                break;
        }

        processItemsDraggables();
        
        if (div.html().length <= 0) {
            var loader = $('#components .busy:first');
            $.ajax({
                url: div.attr('data-url'),
                dataType: 'json',
                beforeSend: function () {
                    loader.show();
                },
                complete: function () {
                    loader.hide();
                },
                success: function(response) {
                    if (response.Success) {
                        div.html(response.html);
                        //console.log(div);
                        currentSection = $('div:visible:first', div)[0].id.length <= 0 ?
                            $(div)[0].id : $('div:visible:first', div)[0].id;
                        //currentSection = $(response.html).get(0).id;
                        $('.sub-menu', div).tabit();
                        //console.log(currentSection);
                    }
                }
            });
        }
    });
    
    $('#virtualMakeover').on('click', '#hairColors li a.change-color', function (e) {
        e.preventDefault();
        
        var $this = $(this);
        var url = $this.attr('data-url');
        var color = vm.global.hair.color;
        var cId = util.urlParam(url, 'c');
        
        if(color != cId) {
            vm.Makeover.changeHairColor({
                url: url,
                beforeSend: function () {
                    mainBusy.show();
                },
                complete: function () {
                    vm.global.hair.color = cId;
                    mainBusy.hide();
                }
            });
        }
    });
    
    $('#virtualMakeover').on('click', '#hairColors a.remove-hair-color', function (e) {
        e.preventDefault();

        //var $this = $(this);

        vm.Makeover.removeHairColor();
    });
    
    var saveCoords = function (data) {
        vm.Trace.saveTraceData({
            data: data,
            url: traceUpdateUrl,
            callback: function(response) {
                if (response.Success) {
                    //console.log('Trace data saved successfully');
                } else {
                    //console.log(response.Message);
                }
            }
        });
    };
    
    var glassesCoordsFirstTimeSave = function () {
        if (!glassTraceSaved && !util.isEmpty(vm.global.photo)) {
            var data = { glass: { coords: vm.global.glassPoints }, fileName: vm.global.photo.fileName };
            saveCoords(data);
        }
    };
    
    var initPhoto = function (url) {
        
        vm.initPhoto({
            url: url, beforeSend: function () { mainBusy.show(); },
            complete: function() {
                mainBusy.hide();
            },
            callback: function (response) {
                processSlider();
                glassTraceSaved = !util.isEmpty(response.Data.glass);

                $('#mainPhotoWrapper .li-controls, #mainPhotoWrapper .controls').show(); //.removeClass('hide'); //.show();
            }
        });
    };

    $('#virtualMakeover').on('click', '.model-photos a, .user-photos a', function(e) {
        e.preventDefault();
        var url = $(this).attr('data-url');
        
        if (util.isEmpty(vm.global.photo)) {
            initPhoto(url);
            photoInit = true;
        } else {
            vm.changePhoto({
                url: url,
                beforeSend: function () { mainBusy.show();},
                complete: function () {
                    mainBusy.hide();
                },
                callback: function (response) {
                    processSlider();
                    glassTraceSaved = !util.isEmpty(response.Data.glass);
                    vm.clearMakeover();
                }
            });
        }
        
        $('#makeoverMainMenu li a[href=#hair]').click();
    });

    $('#virtualMakeover').on('click', '#hairstyles ul.hairstyles li a.retrace, #hairstyles ul.hairstyles li a.try-on',
        function(e) {
            e.preventDefault();

            var $this = $(this);
            var url = $this.attr('href');
            var fn = util.urlParam(url, 'fn');
            //var fileName = vm.global.hair.fileName;

            if (isBefore) {
                afterMakeover();
            }

            if (!util.isEmpty(vm.global.hair) && !util.isEmpty(vm.global.hair.color) && fn == vm.global.hair.fileName) {
                
                vm.Makeover.removeHairColor();
            } else if (util.isEmpty(vm.global.hair) || fn != vm.global.hair.fileName) {

                vm.Makeover.tryOnHairstyle({
                    url: url,
                    beforeSend: function() {
                        mainBusy.show();
                    },
                    complete: function () {
                        var data = vm.global.hair.data;
                        
                        mainBusy.hide();
                        var html = '<b>'+ data.title != null ? data.title : "Untitled hair" + '</b>';
                        
                        var ul = $('#currentlyWearing ul:first');
                        ul.append($('<ul />').html(html));
                        
                        if (!util.isEmpty(calibSaveBtn)) {
                            calibSaveBtn.removeAttr('disabled');
                        }
                    }
                });
            }
        });
    
    $('#virtualMakeover').on('click', '#eyes ul.glasses a.retrace', function (e) {
        e.preventDefault();
        
        var $this = $(this);
        var url = $this.attr('href');
        var fn = util.urlParam(url, 'fn');
        
        if (isBefore) {
            afterMakeover();
        }
       
        if (util.isEmpty(vm.global.glass) || fn != vm.global.glass.getAttr('fileName')) {
            vm.Makeover.tryOnGlass({
                url: $this.attr('href'),
                beforeSend: function() {
                    mainBusy.show();
                },
                complete: function() {
                    mainBusy.hide();
                }
            });
        }
    });
    
    $('#virtualMakeover').on('click', '#eyes ul.contact-lenses a.retrace', function (e) {
        e.preventDefault();

        var $this = $(this);
        var url = $this.attr('href');
        var fn = util.urlParam(url, 'fn');
        var contacts = vm.Makeover.getContacts();
        
        if (isBefore) {
            afterMakeover();
        }

        if (util.isEmpty(contacts) || fn != contacts.left.getAttr('fileName')) {
            vm.Makeover.tryOnContactLenses({
                url: $this.attr('href'),
                beforeSend: function() {
                    mainBusy.show();
                },
                complete: function() {
                    mainBusy.hide();
                }
            });
        }
    });

    $('#virtualMakeover').on('click', '.controls-container .remove-hairstyle, .controls-container .remove-glass, ' +
        '.controls-container .remove-contacts', function (e) {
            e.preventDefault();
            var $this = $(this);

            if ($this.hasClass('remove-hairstyle')) {
                vm.Makeover.removeHairstyle();
            } else if ($this.hasClass('remove-glass')) {
                vm.Makeover.removeGlass();
            } else if ($this.hasClass('remove-contacts')) {
                vm.Makeover.removeContacts();
            }
        });

    $('#HairstyleImage').fileupload({
        dataType: 'json',
        url: $(this).attr('data-url'),
        done: function (e, data) {
            var uploadResponse = data._response.result;

            if (uploadResponse.Success) {
                vm.Calibration.newHair(uploadResponse.imageSrc, uploadResponse.fileName, function (response) {
                    calibSaveBtn.removeAttr('disabled');
                });
            }
        }
    });

    $('#sunglassUpload, #eyeglassUpload').fileupload({
        dataType: 'json',
        url: $(this).attr('data-url'),
        done: function (e, data) {
            var uploadResponse = data._response.result;

            if (uploadResponse.Success) {
                vm.Calibration.newGlass(uploadResponse.imageSrc, function (response) {
                    calibSaveBtn.removeAttr('disabled');
                });
            }
        }
    });
    
    $('#contactLensUpload').fileupload({
        dataType: 'json',
        url: $(this).attr('data-url'),
        done: function (e, data) {
            var uploadResponse = data._response.result;

            if (uploadResponse.Success) {
                $('#contacts').html(uploadResponse.html);
            }
        }
    });
    
    calibSaveBtn.click(function(e) {
        e.preventDefault();
        var section = currentSection;
        
        switch (section) {
            case "hair":
            case "hairstyles":
                var url = null;
                var isNew = (vm.global.hair.saveMode == 'new');
                
                if (isNew) {
                    url = newHairSaveUrl;
                }else {
                    url = $('#editHairUrl').attr('href');
                }
                
                vm.Calibration.saveHair(url, function (response) {
                    if (response.Success) {
                        if(isNew) {
                            $('#hairstyles').html(response.html);
                            var group = vm.global.hair.getParent();
                            group.destroyChildren();
                            group.getLayer().draw();
                        }else {
                            alert('Saved successfully');
                        }
                    }
                });
                break;
            case 'eyeglasses':
                vm.Calibration.saveGlass($('#saveNewEyeglass').attr('href'), function (response) {
                    if (response.Success) {
                        $('#eyeglasses').html(response.html);
                        var group = vm.global.glass.getParent();
                        //group.removeChildren();
                        group.destroyChildren();
                        group.getLayer().draw();
                    }
                });
                break;
            case 'sunglasses':
                vm.Calibration.saveGlass($('#saveNewSunglass').attr('href'), function (response) {
                    if (response.Success) {
                        $('#sunglasses').html(response.html);
                        var group = vm.global.glass.getParent();
                        //group.removeChildren();
                        group.destroyChildren();
                        group.getLayer().draw();
                    }
                });
                break;
        }
    });

    $('body').on('click', '.uploadFileCtrl', function(e) {
        e.preventDefault();

        $($(this).attr('rel')).click();
    });
    
    $('#mainPhotoWrapper .before-after').click(function (e) {
        e.preventDefault();

        if (isBefore) {
            afterMakeover();
        } else {
            beforeMakeover();
        }
    });
    
    //<!-- New photo trace -->
    //var wizardContainer = $('#traceWrapper #traceWizard');
    
    $('#uploadPhoto').click(function (e) {
        e.preventDefault();
        //$('#modalPhotoTip').bPopup();
        
        $('#ImageFile').click();
    });
    
    $('#traceWrapper').on('click', '#traceLipsOpen a', function (e) {

        var $this = $(this);
        if ($this.attr('rel') == 'yes') {
            processTraceZoomSlider();
            vmNewPhotoTrace.global.openLips = true;
        }

        if ($this.attr('rel') == 'no') {
            vmNewPhotoTrace.global.openLips = false;
        }

        $('#step7Next').click();
        e.preventDefault();
    });

    var photoUploadIndicator = $('.upload-instruction .busy:first');
    $('#ImageFile').fileupload({
        dataType: 'json',
        url: $(this).attr('data-url'),
        add: function (e, data) {
            data.context = photoUploadIndicator.show();
            data.submit();
        },
        done: function (e, data) {
            var uploadResponse = data._response.result;
            var container = $('#tracePhoto');

            if (uploadResponse.Success) {
                photoUploadIndicator.show();

                //$('#virtualMakeover').hide();
                
                //$('#traceWrapper').show();
                
                $("#traceOverlay").overlay({
                    position: 'fixed',
                    background: '#000',
                    opacity: 0.5,
                    zIndex: 10000,
                    //url: '',
                    showAfter: 0,
                    hideAfter: 0,
                    close: true,
                    escClose: true,
                    remove: false,
                    replaceWith: ''
                });
                
                //$.blockUI({ message: $('#traceWrapper'), css: { width: '946' } });
                
                //traceWizardUI = uploadResponse.Html;
                $('#traceWrapper .description').html(uploadResponse.Html);
                vmNewPhotoTrace = new VirtualMakeover.Main({
                    container: 'tracePhoto',
                    width: container.width(),
                    height: container.height(),
                    photoSrc: uploadResponse.imgUrl,
                    callback: function () {
                        photoUploadIndicator.hide();

                        //if (uploadResponse.Success) {

                        vmNewPhotoTrace.Trace.init();
                        //$('#virtualMakeover').addClass('hide'); //.hide();
                        //$('#traceWrapper').removeClass('hide'); //.show();

                        if (uploadResponse.Data != null) {
                            traceCoordsData = $.parseJSON(uploadResponse.Data);
                            vmNewPhotoTrace.Trace.assignCoords(traceCoordsData);
                            vmNewPhotoTrace.Trace.drawTraceRectOnFace();
                        } else {
                            initManualTrace();
                            //showWizard(0);
                        }

                        processTraceZoomSlider();
                        vmNewPhotoTrace.global.stage.setDraggable(true);
                        vmNewPhotoTrace.zoom(vmNewPhotoTrace.global.stage.scaleFactor);
                        //}

                    }
                });
                //vmNewPhotoTrace.Trace.init();
            }
        }
    });
    
    var initWizardSettings = function () {
        wizardSettings = {
            ctrls: $("#wizard li"), //Structure for Steps
            nextButtonTitle: 'Next >',
            nextButtonCss: 'btn-next btn btn-default btn-xs',
            previousButtonTitle: '< Back',
            previousButtonCss: 'btn-prev btn btn-default btn-xs',
            finishButtonTitle: 'Save Trace',
            onNext: wizardNext,
            beforeNext: wizardBeforNext,
            onBack: wizardBack,
            onFinish: onWizardFinish
        };
    };
    
    var initManualTrace = function () {

        //wizardContainer.html(traceWizardUI);
        initWizardSettings();
        isManualTrace = true;

        //Start the facial feature calculation and placements
        vmNewPhotoTrace.Trace.initManualTrace(traceClickCount,
            function (anchor) {
                var description = $('p#anchorDescription');
                description.html('');
                
                switch (traceClicked) {
                    case 0:
                        $('#manualAnchor').attr('src', $("#manualTraceRightEyeImg").attr('src'));
                        description.html('Click the right edge of your right eye as shown above');
                    case 1:
                        if (traceClicked == 1) {
                            $('#manualAnchor').attr('src', $('#manualTraceMouthLeftAnchorImg').attr('src'));
                            description.html('Click the left edge of your mouth as shown above');
                        }

                        vmNewPhotoTrace.Trace.eyesManualTraceAnchors.push(anchor);

                        break;
                    case 2:
                        $('#manualAnchor').attr('src', $('#manualTraceMouthRightAnchorImg').attr('src'));
                        description.html('Click the right edge of your mouth as shown above');
                    case 3:
                        if (traceClicked == 3) {
                            $('#manualAnchor').attr('src', $('#manualTraceNoseImg').attr('src'));
                            description.html('Click your nose tip on the picture as shown above');
                        }

                        vmNewPhotoTrace.Trace.mouthManualTraceAnchors.push(anchor);
                        break;
                    case 4:
                        $('#manualAnchor').attr('src', $('#manualTraceAdjustAnchorsImg').attr('src'));
                        description.html('Adjust the anchors to the appropriate location as shown above');
                        vmNewPhotoTrace.Trace.noseManualTraceAnchors.push(anchor);

                        //processTraceZoomSlider();
                        zoomManualTrace();

                        break;
                }
                traceClicked++;

                vmNewPhotoTrace.global.stage.setDraggable(true);
                
                vmNewPhotoTrace.Trace.groups.manualTraceGroup.add(anchor);
                vmNewPhotoTrace.Trace.draw();
            });

        showWizard(0);
    }; 

    var wizardNext = function (i) {
        switch (i) {
            case 0:
                vmNewPhotoTrace.Trace.processAllManualTraceAnchors(function () {
                    vmNewPhotoTrace.Trace.clearLayer();
                    vmNewPhotoTrace.Trace.traceFace();
                    vmNewPhotoTrace.zoomToFace();
                    updateTraceZoomSlider();
                });
                return;
            case 1:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceLeftEye();
                vmNewPhotoTrace.zoomToLeftEye();
                updateTraceZoomSlider();
                return;
            case 2:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceRightEye();
                vmNewPhotoTrace.zoomToRightEye();
                updateTraceZoomSlider();
                return;
            case 3:
                vmNewPhotoTrace.Trace.createEyeballsPointsFromEyesAnchors();
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceLeftEyeball();
                vmNewPhotoTrace.Trace.traceRightEyeball();
                vmNewPhotoTrace.zoomToEyes();
                updateTraceZoomSlider();
                return;
            case 4:
                vmNewPhotoTrace.Trace.createEyeBrowsPointsFromEyesAnchors();
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceLeftEyeBrow();
                vmNewPhotoTrace.Trace.traceRightEyeBrow();
                vmNewPhotoTrace.zoomToEyeBrows();
                updateTraceZoomSlider();
                return;
            case 5:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceMouth();
                vmNewPhotoTrace.zoomToMouth();
                updateTraceZoomSlider();
                $('#step7Next').hide();
                return;
            case 7:
                $('#step9Next').text('Save Trace');

                if (vmNewPhotoTrace.global.openLips) {
                    if (isManualTrace) {
                        vmNewPhotoTrace.Trace.processOpenLipsFromMouthManualTrace();
                    }
                    
                    vmNewPhotoTrace.Trace.clearLayer();
                    vmNewPhotoTrace.Trace.traceOpenLips();
                    vmNewPhotoTrace.zoomToOpenLips();
                    updateTraceZoomSlider();
                    return;
                }
                return 9;
            default:
                return;
        }
    };

    var wizardBack = function (i) {
        switch (i) {
            case 1:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.groups.manualTraceGroup = new Kinetic.Group();
                vmNewPhotoTrace.Trace.layer.add(vmNewPhotoTrace.Trace.groups.manualTraceGroup);

                var anchors = vmNewPhotoTrace.Trace.eyesManualTraceAnchors.concat();
                anchors.push.apply(anchors, vmNewPhotoTrace.Trace.mouthManualTraceAnchors);
                anchors.push.apply(anchors, vmNewPhotoTrace.Trace.noseManualTraceAnchors);

                for (var a = 0; a < anchors.length; a++) {
                    vmNewPhotoTrace.Trace.groups.manualTraceGroup.add(anchors[a]);
                }

                //makeover.mainLayer.draw();
                zoomManualTrace();
                return true;
            case 2:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceFace(false);
                vmNewPhotoTrace.zoomToFace();
                return true;
            case 3:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceLeftEye(false);
                vmNewPhotoTrace.zoomToLeftEye();
                return;
            case 4:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceRightEye(false);
                vmNewPhotoTrace.zoomToRightEye();
                return;
            case 5:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceLeftEyeball(false);
                vmNewPhotoTrace.Trace.traceRightEyeball(false);
                vmNewPhotoTrace.zoomToEyes();
                return;
            case 6:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceLeftEyeBrow(false);
                vmNewPhotoTrace.Trace.traceRightEyeBrow(false);
                vmNewPhotoTrace.zoomToEyeBrows();
                return;
            case 7:
                vmNewPhotoTrace.Trace.clearLayer();
                vmNewPhotoTrace.Trace.traceMouth(false);
                vmNewPhotoTrace.zoomToMouth();
                return;
            default:
                return true;
        }
    };

    var wizardBeforNext = function (i) {

        switch (i) {
            case 0:
                //Make sure all anchors for this step have been added to photo if manual tracing is enabled
                if (isManualTrace && traceClicked < traceClickCount) {
                    alert('All trace anchors have not been added. Please click on the photo to add anchor.');
                    return false;
                }
                return true;
            case 7:
            case 8:
                if (!vmNewPhotoTrace.global.openLips) {
                    vmNewPhotoTrace.Trace.clearLayer();
                }
                vmNewPhotoTrace.zoom(vmNewPhotoTrace.global.stage.scaleFactor);
                updateTraceZoomSlider();
                return true;
                break;
            case 9:
                onWizardFinish();
                return false;
            default:
                return true;
                break;
        }
    };

    var zoomManualTrace = function () {
        //zoom
        var anchors = util.clone(vmNewPhotoTrace.Trace.eyesManualTraceAnchors);
        anchors.push.apply(anchors, util.clone(vmNewPhotoTrace.Trace.mouthManualTraceAnchors));
        anchors.push.apply(anchors, util.clone(vmNewPhotoTrace.Trace.noseManualTraceAnchors));

        var rect = util.anchorsToRect(anchors);

        rect.x -= (rect.width / 4);
        rect.y -= (rect.height / 4);
        rect.width += (rect.width / 2);
        rect.height += (rect.height / 2);

        vmNewPhotoTrace.zoomToRoi(rect);
        updateTraceZoomSlider();
        //console.log('updated zoom');
    };

    var onWizardFinish = function () {
        var saveUrl = $('#saveTrace').attr('href');

        var process = function (html, coordData) {
            $("#traceOverlay").overlay({ remove: true });
            //$('#traceWrapper').hide();
            $('.traced-photos .user-photos').html(html);
            //$('#virtualMakeover').show();
            $('#tracePhotoWrapper #tracePhoto').html("");
            $('#traceWrapper .description').html("");
            //$('#traceWrapper #autoTraceWizard').html("");
            //traceWizardUI = null;
            delete traceWizard;
            //var imgSrc = vmNewPhotoTrace.global.photo.getImage().src;
            vmNewPhotoTrace = null;
            traceClicked = 0; //keeps track of how time the image has been clicked to add manual trace anchor
            vm.clearMakeover();

            $('.traced-photos .user-photos a.thumbnail:first').click();
            
            //vm.setPhoto(imgSrc, coordData, function () {
            //    processSlider();
            //    glassTraceSaved = !util.isEmpty(coordData.glass);
            //    $('#mainPhotoWrapper .li-controls, #mainPhotoWrapper ul.controls').show();
            //});
        };

        var saveUiUpdate = function(isProcessing) {
            var saveBtn = $('#step9Next');
            var prevBtn = $('#step9Prev');
            //var parent = $('#step9');
            var busy = $('#traceWrapper .busy');
            
            if (isProcessing) {
                saveBtn.attr('disabled', 'disabled');
                prevBtn.attr('disabled', 'disabled');
                busy.show();
            }else {
                saveBtn.removeAttr('disabled');
                prevBtn.removeAttr('disabled');
                busy.hide();
            }
        };
        
        if (isManualTrace) {
            vmNewPhotoTrace.Trace.saveAllTraceAnchors({
                url: saveUrl,
                beforeSend: function () { saveUiUpdate(true);},
                //error: saveUiUpdate(false),
                callback: function (response) {
                    saveUiUpdate(false);
                    
                    if (response.Success) {
                        process(response.Data, response.coordData);
                    }
                }
            });
        } else {

            if (vmNewPhotoTrace.openLips) {
                //open lips
                var opnLips = vmNewPhotoTrace.Trace.convertAnchorsToCoordsData(vmNewPhotoTrace.global.openLipsAnchors);
                traceCoordsData.openLips = opnLips;
            }

            vmNewPhotoTrace.Trace.saveTraceData({
                data: traceCoordsData,
                url: saveUrl,
                beforeSend: function () { saveUiUpdate(true);},
                //error: function () { saveUiUpdate(false); },
                callback: function (response) {
                    saveUiUpdate(false);
                    
                    if (response.Success) {
                        process(response.Data, traceCoordsData);
                    }
                }
            });
        }
    };

    $('#traceWrapper').on('click', '#autoTrace a', function (e) {
        e.preventDefault();

        var $this = $(this);

        if ($this.attr('rel') == 'yes') {
            $('#traceWrapper #wizard').show();
            initWizardSettings();
            
            vmNewPhotoTrace.global.openLips = false;
            showWizard(9);
            $('#step9Next').html('Save Trace');
            //$('#step7Next').hide();
        }

        if ($this.attr('rel') == 'no') {
            initManualTrace();
        }
    });

    var showWizard = function (initStep) {

        wizardSettings.initStep = initStep;
        $('#autoTrace').hide();
        $('#wizard').show();

        traceWizard = new VirtualMakeover.TraceWizard(wizardSettings);
    };

    $('body').on('click', '#traceWrapper .dialog-close', function(e) {
        e.preventDefault();
        
        //$('#traceWrapper').hide();
        //$('#virtualMakeover').show();
        $("#traceOverlay").overlay({ remove: true });
        $('#tracePhotoWrapper #tracePhoto').html("");
        $('#traceWrapper .description').html("");
        traceClicked = 0; //keeps track of how time the image has been clicked to add manual trace anchor
        
        delete traceWizard;
        vmNewPhotoTrace = null;
    });

    $('#virtualMakeover').on('click', '#hairstyles .item-content .pagination a[href]', function(e) {
        e.preventDefault();
        var elem = $(this);
        
        $.ajax({
            url: elem.attr('href'),
            dataType: 'json',
            error: function (error) {
                alert(error);
            },
            success: function (response) {
                if(response.Success) {
                    $('#hairstyles .item-content').html(response.html);
                }
            }
        });
    });
});