// Общие скрипты сайта

// Подтверждение для форм с data-confirm
document.addEventListener('submit', function (event) {
    const form = event.target.closest('form[data-confirm]');
    if (form && !window.confirm(form.dataset.confirm)) {
        event.preventDefault();
    }
});
