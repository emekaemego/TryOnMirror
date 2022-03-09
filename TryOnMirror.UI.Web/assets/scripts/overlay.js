// Overlay js - lightweight jquery overlay plugin -version 2.0 - http://dev.codinglog.com/jquery/overlay-js/
// Copyright (c) 2011 Sandeep MT - http://mtsandeep.com
// Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
;(function($){ 
	$.fn.extend({  
		overlay: function(options) {

			var defaults = { 
				position: 'fixed',
				background: '#000',
				opacity:0.5,
				zIndex:99,
				url: '',
				showAfter: 0,
				hideAfter: 0,
				close: true,
				escClose: true,
				remove: false,
				replaceWith: '',
				onTrigger : function(){},
				onLoad : function(){},
				onClose : function(){}
			};
			  
			var options = $.extend(defaults, options); 
			return this.each(function() { 
				var o = options; 

				var $this = $(this); // div with the content of overlay
				o.onTrigger($this); //callback at the start of overlay
				var pageCover = '<div id="cover" style="position:fixed;top:0;left:0;width:100%;height:100%;background-color:'+o.background+';zoom:1;filter:alpha(opacity='+o.opacity*100+');opacity:'+o.opacity+';z-index:'+o.zIndex+';"></div>';
				var close ='<a class="overlay-close" href="#">X</a>';
				
				
				if(o.replaceWith){ //checking if its loading an overlay into another overlay.
					if ($('.overlay').is(':visible')) {
						$('.overlay').removeAttr('style').removeClass('overlay').children('.overlay-close').hide();
						var $this = $(o.replaceWith);
						loadContent();
					}
				} else {
					
					if(o.showAfter) {
						setTimeout(function() {
							showOverlay();
						}, o.showAfter);
					} else {
						showOverlay();
					}
					
					if(o.hideAfter){
						 setTimeout(function() {
							removeOverlay();
						  }, o.showAfter+o.hideAfter);
					} else if(o.remove){
						removeOverlay();
					}
				}
				
				if(o.escClose) {
					$(document).bind('keyup.overlay', function(e){
					   if (e.which == 27) {
							removeOverlay();
					   }
					});
				}
				
				$this.delegate('.overlay-close','click', function(e) { //replacing live with delegate to support v1.7
					e.preventDefault();
					removeOverlay();
				});
				
					on_resize(function(){
						fixMessage();
					});
				
				function showOverlay(){
					if ($('#cover').length > 0){
						$('#cover').css({'background-color':o.background,'opacity':o.opacity,'filter':'alpha(opacity ='+o.opacity*100+')'}).show(); //adding new background and opacity if it already exists
					} else {
						$('body').append(pageCover);
					}
					loadContent();
				}
				function loadContent(){
					
					if(o.url){ //check if url is provided, if yes load that to the div
						$this.load(o.url,function(){
							fixMessage();
						});
					}
					$this.css('z-index',o.zIndex+9).addClass('overlay');
					fixMessage();
					$this.fadeIn(500,function(){
						o.onLoad($this); //callback after overlay is loaded and displayed
					}); //show the div
					
				}
				
				function showClose(){
					if(o.close){
						if ($this.children('.overlay-close').length){
							$this.children('.overlay-close').show();
						} else {
							$this.prepend(close);
						}
					}
				}
				
				function fixMessage(){
					var remainingHeight = $(window).height() - $this.outerHeight();
					var remainingWidth = $(window).width() - $this.outerWidth();
					if(remainingHeight < 40){
						$this.css({'position':'absolute','top': $(window).scrollTop()+20+'px'});
					} else if(o.position=='fixed'){
						$this.css({'position':o.position,'top': remainingHeight / 2 + 'px'});
					} else {
						$this.css({'position':o.position,'top': (remainingHeight / 2) + $(window).scrollTop() + 'px'});
					}
					if(remainingWidth < 40){
						$this.css('left','20px');
					} else {
						$this.css('left', (remainingWidth / 2) + $(window).scrollLeft() + 'px');
					}
					showClose();
				}
				
				// resize with delay
				function on_resize(c,t){
					$(window).bind('resize.overlay',function(){ 
						clearTimeout(t);	
						t = setTimeout(c, 100); 
					});
				}
					
				function removeOverlay(){
					$this.children('.overlay-close').hide();
					$this.removeAttr('style').removeClass('overlay');
					$(window).unbind('resize.overlay');
					$(document).unbind('keyup.overlay');
					if(o.url){
						$this.empty();
					}
					$('#cover').fadeOut(200,function(){
						o.onClose($this); //callback after overlay is removed
						$this.unbind(); //unbind everything on the element
					});
				}
			
			}); 
		} 
	}); 
})(jQuery);