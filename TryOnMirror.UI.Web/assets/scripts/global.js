var stack_topleft = { "dir1": "down", "dir2": "right", "push": "top" };
var stack_bottomleft = { "dir1": "right", "dir2": "up", "push": "top" };
var stack_custom = { "dir1": "right", "dir2": "down" };
var stack_custom2 = { "dir1": "left", "dir2": "up", "push": "top" };
var stack_bar_top = { "dir1": "down", "dir2": "right", "push": "top", "spacing1": 0, "spacing2": 0 };
var stack_bar_bottom = { "dir1": "up", "dir2": "right", "spacing1": 0, "spacing2": 0 };

(function ($) {
    $.fn.extend({
        tabit: function (callback) {
            $(this).each(function () {
                // For each set of tabs, we want to keep track of
                // which tab is active and its associated content
                var $active, $content, $links = $(this).find('a');

                // If the location.hash matches one of the links, use that as the active tab.
                // If no match is found, use the first link as the initial active tab.
                $active = $($links.filter('[href="' + location.hash + '"]')[0] || $links[0]);
                $active.addClass('active');
                $content = $($active.attr('href'));

                // Hide the remaining content
                $links.not($active).each(function () {
                    $($(this).attr('href')).hide();
                });

                // Bind the click event handler
                $(this).on('click', 'a', function (e) {
                    // Make the old tab inactive.
                    $active.removeClass('active');
                    $content.hide();

                    // Update the variables with the new link and content
                    $active = $(this);
                    $content = $($(this).attr('href'));

                    // Make the tab active.
                    $active.addClass('active');
                    $content.show();
                    
                    // Prevent the anchor's default click action
                    e.preventDefault();
                });
            });
        }
    });
})(jQuery);

(function ($) {
    $.override = { 'show': $.fn.show, 'hide': $.fn.hide };

    $.each($.override, function (M, F) {

        var m = M.replace(/^\w/, function (r) { return r.toUpperCase(); });

        $.fn[M] = function(speed, easing, callback) {

            var args = [speed || 0, easing || '', callback || function() {
            }];

            if ($.isFunction(speed)) {
                args[2] = speed;
                args[0] = 0;
            } else if ($.isFunction(easing)) {
                args[2] = easing;
                args[1] = '';
            }

            if (!this.selector) {
                F.apply(this, arguments);
                return this;
            }

            return this.each(function() {
                var obj = $(this),
                    oldCallback = args[args.length - 1],
                    newCallback = function() {
                        if ($.isFunction(oldCallback)) {
                            oldCallback.apply(obj);
                        }
                        obj.trigger('after' + m);
                    };

                obj.trigger('before' + m);
                args[args.length - 1] = newCallback;

                //alert(args);
                F.apply(obj, args);

            });
        };
    });
})(jQuery);

(function ($) {
    $.urlParam = function (url, name) {
        var results = new RegExp('[\\?&#]' + name + '=([^&#]*)').exec(url);
        if (!results) {
            return '';
        }
        return results[1] || '';
    };
})(jQuery);

jQuery.fn.mousehold = function(timeout, f) {
    if (timeout && typeof timeout == 'function') {
        f = timeout;
        timeout = 100;
    }
    if (f && typeof f == 'function') {
        var timer = 0;
        var fireStep = 0;
        return this.each(function() {
            jQuery(this).mousedown(function() {
                fireStep = 1;
                var ctr = 0;
                var t = this;
                timer = setInterval(function() {
                    ctr++;
                    f.call(t, ctr);
                    fireStep = 2;
                }, timeout);
            });

            var clearMousehold = function() {
                clearInterval(timer);
                if (fireStep == 1) f.call(this, 1);
                fireStep = 0;
            };

            jQuery(this).mouseout(clearMousehold);
            jQuery(this).mouseup(clearMousehold);
        });
    }
};

$(document).ready(function () {
    $.pnotify.defaults.history = false;
    
    (function ($) {
        $.fn.ajaxFormSubmit = function (options) {

            $(document).on('submit', this.selector, function () {

                var $el = $(this);

                var opts = $.extend({
                    url: $el.attr('action'),
                    type: $el.attr('method'),
                    data: $el.serialize(),
                    dataType: 'json',
                    cache: false,
                    iframe: false,
                    callback: function (response) {},
                    //beforeSend: function(){}
                }, options);

                $el.ajaxSubmit({
                    dataType: opts.dataType,
                    iframe: opts.iframe,
                    
                    //beforeSend: function () {
                    //    opts.callback.call($el);
                    //},
                    error: function (errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (response) {
                        opts.callback.call($el, response);
                    }
                });

                return false;
            });
        };
    })(jQuery);
       
    $('#hair-categories a.delete, #hair-categories a.edit').css('display', 'none');
    
    $('#body').on('mouseover', '#hair-categories li', function () {
        $('a.delete, a.edit', $(this)).show();
    });
    
    $('#body').on('mouseleave', '#hair-categories li', function () {
        $('a.delete, a.edit', this).hide();
    });

    $('#body').on('click', 'li a.delete-li', function (e) {
        e.preventDefault();
        var $this = $(this);

        var message = $this.attr('data-delete-message'); //"Are you sure you want to delete this record?";

        //if ($this.hasAttr('rel'))
          //  message = $this.attr('rel');
        
        //Confirm(message, function (result) {
            if (confirm(message)) {
                $.post($this.attr('href'), function (response) {
                    if (response.Success) {
                        $this.parents('li:first').remove();
                        $.pnotify({ text: response.Message, type: 'success', history: false });
                    }
                });
            }
        });
    //});
    
    $('#body').on('click', 'a[data-modal="modal"]', function (e) {
        e.preventDefault();

        var el = $(this);

        $.ajax({
            dataType: 'json',
            url: el.attr('href'),
            beforeSend: function () {
                $('<span />', { 'class': 'busy' }).insertAfter(el);
            },
            complete: function () {
                el.next('span.busy').remove();
            },
            success: function (response) {
                if (response.Success) {
                    if (response.Data !== null || response.Data !== undefined) {
                        el.next('span.busy').remove();
                        var modal = $('#ajax');
                        
                        modal.html(response.Data);
                        
                        modal.bPopup({
                            closeClass: 'dialog-close',
                            modalClose: false,
                            fadeSpeed: 'slow',
                            followSpeed: 1500,
                            position: ['auto', 'auto'],
                            opacity: 0.3,
                        });
                        
                        //$.pnotify({ text: 'Loaded successfully', type: 'success', icon: 'icon-checkmark-3' });
                    }
                }
            }
        });
    });

    $('form[data-ajax-enabled="true"]').ajaxFormSubmit({
        callback: function (response) {
            var form = $(this);
            
            if (response.Success) {
                var insertType = form.attr('data-insert-type');
                
                if (!util.isEmpty(insertType) || insertType == "prepend") {
                    $(form.attr('data-target')).prepend(response.Html);
                }else {
                    $(form.attr('data-target')).append(response.Html);
                }
                
                form.resetForm();
                $.pnotify({ text: response.Message, type: 'success', icon: 'icon-checkmark-3' });
            } else {
                alert(response.Message);
            }
        }
    });
    
    $('form[data-form-type="edit"]').ajaxFormSubmit({
        callback: function (response) {
            var form = $('form:first', $(this));

            if (response.Success) {
                $.pnotify({ text: response.Message, type: 'success', icon: 'icon-checkmark-3' });
            } else {
                if (response.Html) {
                    form.removeData("validator");
                    form.removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse(form);
                    form.replaceWith(response.Html);
                }else {
                    alert(response.Message);
                }
            }
        }
    });

    $('form#commentForm').ajaxFormSubmit({
        callback: function (response) {
            var form = $(this);

            if (response.Success) {
                $(form.attr('data-target')).append(response.Html);

                form.resetForm();
            } else {
                alert(response.Message);
            }
        }
    });
    
    $('#thumbImageForm').ajaxFormSubmit({
        //dataType: 'json',
        ////url: $(this).attr('data-url'), // + "&_" + new Date().toTimeString(),
        //error: function (err) {
        //    alert("Error occured: " + err);
        //},
        callback: function (response) {
            if (response.Success) {
                var img = $('<img />', { src: response.imageSrc });
                $('#imageZone').html(img);
                applyJcrop(img);
            }
        }
    });
    
    $('#makeoverSalons').ajaxFormSubmit({
        callback: function (response) {
            var form = $(this);

            if (response.Success) {
                var target = $(form.attr('data-target'));
                
                target.html(response.Html);
                var link = $('a.book-hairstyle', target);
                link.click(function(e) {
                    e.preventDefault();

                    $.ajax({
                        url: link.attr('href'),
                        dataType: 'json',
                        success: function (result) {
                            if (result.Success) {
                                var formDivs = $('div.form', target);
                                formDivs.hide('fast');
                                formDivs.html();

                                var activeDiv = $('div.form', link.parents('div.salon:first'));
                                activeDiv.html(result.Html);
                                //$.validator.unobtrusive.parse($('form', activeDiv));
                                
                                $('#BookingDate', activeDiv).datetimepicker({
                                    timeFormat: "hh:mm tt",
                                    dateFormat: "dd/mm/yy",
                                    minDateTime: 0
                                });
                                activeDiv.show('slow');
                            }
                        }
                    });
                });
            } else {
                form.parents("div:first").html(response.Html);
            }
        }
    });

    $(document).on('submit', '#haistyleBookingForm', function (e) {
        e.preventDefault();
        
        var $el = $(this);
        var hair = vm.global.hair;
        
        if (!util.isEmpty(hair)) {
            //var stage = vm.global.stage;
            $('#HairstyleFileName').val(hair.fileName);
            $('#PhotoFileName').val(vm.global.photo.fileName);
            
            vm.toDataURL(function (img) {

                $('#ImageData').val(img.replace('data:image/png;base64,', ''));
                
                $el.ajaxSubmit({
                    dataType: 'json',

                    error: function (errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (response) {
                        if(response.Success) {
                            alert('Hairstyle booked successfully');
                        }else {
                            alert(response.Message);
                        }
                    }
                });
            });
        } else {
            alert('No hairstyle is currently selected');
            return false;
        }       

        return false;
    });
    
});

(function($) {
    $.fn.top = function(options) {

    };
})(jQuery);

function show_stack_bar_top(title, text,type) {
    var opts = {
        title: "Over Here",
        text: "Check me out. I'm in a different stack.",
        addclass: "stack-bar-top",
        cornerclass: "",
        width: "100%",
        stack: stack_bar_top
    };
    switch (type) {
        case 'error':
            opts.title = "Oh No";
            opts.text = "Watch out for that water tower!";
            opts.type = "error";
            break;
        case 'info':
            opts.title = "Breaking News";
            opts.text = "Have you met Ted?";
            opts.type = "info";
            break;
        case 'success':
            opts.title = "Good News Everyone";
            opts.text = "I've invented a device that bites shiny metal asses.";
            opts.type = "success";
            break;
    }
    $.pnotify(opts);
}