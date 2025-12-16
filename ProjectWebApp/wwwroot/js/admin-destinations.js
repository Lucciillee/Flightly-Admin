// ================================
// ADMIN DESTINATIONS
// ================================

function previewPopupImage(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            const preview = document.getElementById("popupPreview");
            if (preview) {
                preview.src = e.target.result;
            }
        };

        reader.readAsDataURL(input.files[0]);
    }
}
