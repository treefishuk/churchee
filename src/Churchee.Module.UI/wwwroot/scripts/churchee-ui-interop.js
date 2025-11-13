globalThis.localStorageHelper = {
    save: function (key, data) {
        localStorage.setItem(key, JSON.stringify(data));
    },
    load: function (key) {
        return JSON.parse(localStorage.getItem(key));
    }
};

globalThis.getImagePreview = (file) => {
    return new Promise((resolve) => {
        const reader = new FileReader();
        reader.onload = (e) => resolve(e.target.result);
        reader.readAsDataURL(file);
    });
};