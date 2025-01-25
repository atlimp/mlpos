class ImagePreview {
    constructor({ imageInputId, imagePreviewClass }) {
        this.imagePreviews = Array.from(document.querySelectorAll(imagePreviewClass));
        this.imageInput = document.querySelector(imageInputId);

        if (this.imageInput) {
            this.imageInput.addEventListener('change', (e) => {
                e.preventDefault();
                this.setImagePreview(e.target);
            });
        }
    }

    setImagePreview(e) {
        if (e.files && e.files[0]) {
            const reader = new FileReader();
            reader.onload = (readerEvent) => {
                this.imagePreviews.forEach(element => element.src = readerEvent.target.result);
            };

            reader.readAsDataURL(e.files[0]);
        }
    }
}