
	function setIPhoneMenu(){
		
		jQuery('.subItem').remove();
		jQuery(".iphonNav").css("display", "none");
		jQuery(".iphonNav").addClass("iphon_navigation");
				jQuery(".iphonNav").removeClass("menu");
				jQuery(".iphonNav > ul").attr("id","");
				jQuery(".menuImage").css("display","block");
				jQuery(".iphonNav ul li").each(function(){
					if(jQuery(this).children("ul").length >0)
					{
						jQuery(this).addClass("parentMenu");
						jQuery(this).prepend("<div class='subItem'></div>");
						jQuery(this).children("ul").addClass("subMenu");
					}
				});
				jQuery(".subMenu").css("display","none");
				jQuery(".menuImage img").unbind("click");
				jQuery(".menuImage img").click(function(){
					jQuery(".iphonNav").slideToggle(300, function(){
						if(!jQuery(this).is(":visible")){
							jQuery(this).find(".subMenu").each(function(){
								jQuery(this).css("display","none");
								jQuery(this).parent().removeClass("parentMenuActive");
							});
						}
					});
				});
				jQuery(".subItem").unbind("click");
				jQuery(".subItem").click(function(e){
					jQuery(this).parent().children(".subMenu").slideToggle(300, function(){
						if(jQuery(this).is(":visible")){
								jQuery(this).parent().parent().children(".parentMenuActive").find(".subMenu").each(function(){
										jQuery(this).slideUp(300);
										jQuery(this).css("display","none");
										jQuery(this).parent().removeClass("parentMenuActive");
									});
								jQuery(this).parent().parent().children(".parentMenuActive").removeClass("parentMenuActive");
							jQuery(this).parent().addClass("parentMenuActive");
							
						}else{
							jQuery(this).parent().removeClass("parentMenuActive");
							jQuery(this).find(".subMenu").each(function(){
								jQuery(this).css("display","none");
								jQuery(this).parent().removeClass("parentMenuActive");
								
							});
						}
					});
					
					e.stopPropagation();
				});
	}
	function setDesktopMenu(){
		jQuery(".iphonNav ul li").find(".subMenu").each(function(){
			jQuery(this).css("display","none");
			jQuery(this).parent().removeClass("parentMenuActive");
		});
		jQuery(".iphonNav").addClass("menu");
		jQuery(".iphonNav").css("display", "block");
		jQuery(".iphonNav").removeClass("iphon_navigation");
		jQuery(".iphonNav>ul").attr("id","nav");
		jQuery(".menuImage").css("display","none");
	}
	jQuery(document).ready(function(){
		if(jQuery(window).width()<767)
		{
			setIPhoneMenu();
		}else{
			setDesktopMenu();
		}
		
	});
	jQuery(window).resize(function(){
		if(jQuery(window).width()<767)
		{
			setIPhoneMenu();
		}else{
			setDesktopMenu();
		}
	});
	
	
	jQuery(".iphon_navigation ul li a").click(function(e){
			jQuery(".iphon_navigation").css("display", "none");
	});

