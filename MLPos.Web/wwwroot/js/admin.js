class AdminHandlers {
    constructor() {

    }

    initEventHandlers() {
        const navbar = document.querySelector('#navbar');
        document.querySelector('#hamburger').addEventListener('click', (e) => {
            this.toggleVisible(navbar);
        });
    }

    toggleVisible(e) {
        e.classList.toggle('hidden');
    }
}

(() => {
    (new AdminHandlers()).initEventHandlers();
})()