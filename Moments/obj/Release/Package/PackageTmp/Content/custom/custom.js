$(document).ready(function () {
    $('.create_story').hide();
    $(".submit_text").click(function () {
        $('.createMoment').fadeOut();
        $('.m1_add_icon').fadeOut();
        $(".moment1_text").show();
        $(".moment_1 .moment_text").show();
        var x = $('.froala-editor').froalaEditor('html.get');
        $(".moment1_text").append(x);
        $(".moment_1").append('<button type="button" class="btn btn-info btn-lg add_icon m2_add_icon" data-toggle="modal" data-target="#myModal2"><img src="images/add_icon.png"></button><button type="button" class="btn btn-info btn-lg add_icon narration1_add_icon" data-toggle="modal" data-target="#myModal6"><img src="images/add_icon.png"></button>');
    });
    $('.add_text').click(function () {
        $('.modal').fadeOut();
        $('.modal-backdrop').fadeOut();
        $('body').removeClass("modal-open");
        $('.createMoment').show();

    });
    $('.add_image').click(function () { $('.imgupload').trigger('click'); });
    //$('.add_image_2').click(function(){ $('.imgupload2').trigger('click'); });
    //$('.add_image_3').click(function(){ $('.imgupload3').trigger('click'); });
    //$('.add_image_4').click(function(){ $('.imgupload4').trigger('click'); });
    //$('.add_image_5').click(function(){ $('.imgupload5').trigger('click'); });
    //$('.add_image_6').click(function(){ $('.imgupload6').trigger('click'); });
    $('.show-placeholder').hide();
    var currentZoom = 1.0;
    $('.zoom_in').click(
        function () {
            $('.m_tool_container').animate({ 'zoom': currentZoom += .2 }, 'fast');
        })
    $('.zoom_out').click(
        function () {
            $('.m_tool_container').animate({ 'zoom': currentZoom -= .2 }, 'fast');
        });
});