function FullPopUp(pURL, pWindowName) {
    // % of screen resolution
    let popupScreenSize = 0.65;

    let customizedWidth = screen.width * popupScreenSize;
    let customizedHeight = screen.height * popupScreenSize;

    APopUp = window.open(pURL, pWindowName, 'menubar=yes,toolbar=yes,location=no,status=no,width=' + customizedWidth + ',height=' + customizedHeight + ',scrollbars=yes,resizable=yes');
}

function FullHelpPopUp(pURL, pWindowName, title) {
    $.ajax({
        url: pURL,
        success: function (result) {
            // setting default style is rightslink style
            jQuery.prompt.setDefaults({prefix: 'rightslink'});

            var helpPrompt = {
                states: {
                        title: title,
                        buttons: {OK: true},
                        html: result
                    }
            };

            $.prompt(helpPrompt);
        },
        error: function () {
            $.prompt("Error has occurred. Please try later.", {buttons: {OK: true}});
        }
    })
}

function FullPreviousCommentsPopUp(pURL, pWindowName) {
    APopUp = window.open(pURL, pWindowName, 'menubar=no,location=no,toolbar=no,status=no,width=720,height=400,scrollbars=yes,resizable=yes');

}
