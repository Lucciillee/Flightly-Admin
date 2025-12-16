// ================================
// ADMIN DESTINATIONS - CREATE PAGE
// ================================

function previewDestinationImage(input) {
    if (!input.files || !input.files[0]) return;

    const reader = new FileReader();

    reader.onload = function (e) {
        const img = document.getElementById("previewImage");
        const placeholder = document.getElementById("previewPlaceholder");

        if (img) {
            img.src = e.target.result;
            img.style.display = "block";
        }

        if (placeholder) {
            placeholder.style.display = "none";
        }
    };

    reader.readAsDataURL(input.files[0]);
}
