// Notifications
(function($){
	'use strict';

	// Define plugin name and parameters
	$.fn.notifyMe = function ($position, $type, $title, $content, $velocity, $PandingReviewList) {
		
		// Remove recent notification for appear new
		$('.notify').remove();

		// Create the content of Alert
		var close = "<a class='notify-close'>x</a><br/>";
		var header = "<section class='notify' data-position='"+ $position +"' data-notify='" + $type + "'>" + close + "<h4>" + $title + "</h4>";
		var content = "<div class='notify-content'>" + $content + "</div>";
		//var tblProject = "<br/><div><table class='display postable table table-stats table-condensed dataTable no-footer'><tr><th>Project Name</th></tr >" + $PandingReviewList + "</table ></div></section>";
		var tblProject = "<br/><div class='reviewsidebar'>" + $PandingReviewList+"</div></section>";

		var notifyModel = header + content + tblProject;

		$('body').prepend(notifyModel);

		var notifyHeigth = $('.notify').outerHeight();

		// Show Notification

		if($position == "bottom"){
			$('.notify').css('bottom', '-' + notifyHeigth);
			$('.notify').animate({
				bottom: '0px'
			},$velocity);
		}

		else if($position == "top"){
			$('.notify').css('top', '-' + notifyHeigth);
			$('.notify').animate({
				top: '0px'
			},$velocity);
		}

		else if($position == "right"){
			$('.notify').css('right', '-' + notifyHeigth);
			$('.notify').animate({
				right: '0px'
			},$velocity);
		}

		else if($position == "left"){
			$('.notify').css('left', '-' + notifyHeigth);
			$('.notify').animate({
				left: '0px'
			},$velocity);
		}

		// click href link
		$('#closureReviewlink').click(function () {
			$('.notify-close').click();
		});

		// Close Notification
		$('.notify-close').click(function(){
			// Move notification
			if($position == "bottom"){
				$(this).parent('.notify').animate({
					bottom: '-' + notifyHeigth
				},$velocity);
			}
			else if($position == "top"){
				$(this).parent('.notify').animate({
					top: '-' + notifyHeigth
				},$velocity);
			}
			else if($position == "right"){
				$(this).parent('.notify').animate({
					right: '-' + notifyHeigth
				},$velocity);
			}
			else if($position == "left"){
				$(this).parent('.notify').animate({
					left: '-' + notifyHeigth
				},$velocity);
			}

			// Remove item when close
			setTimeout(function(){
				$('.notify').remove();
			},$velocity + 200);

		});
	}
}(jQuery));






