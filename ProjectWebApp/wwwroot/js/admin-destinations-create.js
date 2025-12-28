// ================================
// ADMIN DESTINATIONS - CREATE PAGE
// ================================

function previewDestinationImage(input) {
    // If there are no files selected, exit the function
    if (!input.files || !input.files[0]) return;

    // Create a new FileReader object to read the file contents
    const reader = new FileReader();

    // This function runs once the file has been fully read (converted to a Data URL)
    reader.onload = function (e) {
        // Get the <img> element that will display the preview
        const img = document.getElementById("previewImage");

        // Get the placeholder element that should be hidden when an image is shown
        const placeholder = document.getElementById("previewPlaceholder");

        // Set the <img> src to the loaded file data (base64 Data URL) and make it visible
        if (img) {
            img.src = e.target.result;// The full image data
            img.style.display = "block";
        }

        // Hide the placeholder since an actual image is now being displayed
        if (placeholder) {
            placeholder.style.display = "none";
        }
    };

    // Start reading the selected file as a Data URL (converts the image to base64 for preview)
    reader.readAsDataURL(input.files[0]);
}
