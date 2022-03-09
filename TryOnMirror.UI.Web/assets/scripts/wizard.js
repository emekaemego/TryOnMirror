//Website: http://techbrij.com
//Live Demo: http://demo.techbrij.com/865/demo-jquery-wizard.php

var wizard = function(settings) {
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

            //if (title == undefined || (title != undefined && settings.validateWizardByTitle(title))) {
            //    if (settings.validateWizardStep != undefined && settings.validateWizardStep(i)) {

            //        var nextStep = settings.callBack(i);

            //        $("#" + stepName).hide();

            //        if (nextStep == null || nextStep == false) {
            //            $("#step" + (i + 1)).fadeIn(1500); //Default is next li will be displayed on Next Click
            //        } else {
            //            $("#step" + nextStep).fadeIn(1500); //Jump Wizard to go to particular index
            //        }
            //        arrPos.push(i);
            //        //$("#step" + (i + 1)).slideDown(1500);
            //    }
            //}
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
        var prevBtn = $('<a />', { text: settings.finishButtonTitle, 'class': settings.nextButtonCss, 'id': stepName + "Next" });

        $("#" + stepName + "commands").append(prevBtn);
        $("#" + stepName + "Next").bind("click", function(e) {
            //$("#" + stepName).hide();
            //$("#step" + (i + 1)).fadeIn(2500);
            //$("#step" + (i + 1)).slideDown(1500);
            settings.onFinish(i);
        });

    }

//steps.show();
};

//var wizSettings = function() {
//    return { ctrls: null, nextButtonTitle: 'Next >', nextButtonCss: 'next', previousButtonTitle: 'Back', finishButtonTitle: 'Start Makeover', callBack: null, validateWizardStep: true, validateWizardByTitle: true, initStep: false };
//};