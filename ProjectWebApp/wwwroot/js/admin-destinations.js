// ================================
// ADMIN DESTINATIONS
// ================================

function previewPopupImage(input) {
    // Check if the input contains any files and pick the first one
    if (input.files && input.files[0]) {
        const reader = new FileReader();//create a FileReader object

        // Define what happens when the file is fully loaded
        reader.onload = function (e) {
            // Find the <img> element where the preview will be displayed
            const preview = document.getElementById("popupPreview");
            if (preview) {
                preview.src = e.target.result;// Set the preview image source
            }
        };

        reader.readAsDataURL(input.files[0]);//Converts the file into a base64 URL so the <img> can display it.
    }
}
