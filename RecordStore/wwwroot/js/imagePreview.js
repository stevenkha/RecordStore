document.getElementById('imageFile').addEventListener('change', function (event) {
    const preview = document.getElementById('previewImage');
    const previewDiv = document.getElementById('imagePreview');

    if (event.target.files && event.target.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            previewDiv.style.display = 'block';
        }

        reader.readAsDataURL(event.target.files[0]);
    }
});