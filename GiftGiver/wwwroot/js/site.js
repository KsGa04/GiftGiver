document.querySelector('.v133_370').addEventListener('click', function () {
    var input = document.createElement('input');
    input.type = 'file';
    input.accept = '.png, .jpeg, .jpg';
    input.onchange = function (e) {
        var file = e.target.files[0];

        var formData = new FormData();
        formData.append('file', file);

        fetch('/Controllers/HomeController/ChangeImage', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                // Обработка успешного ответа от сервера
                console.log(data);
            })
            .catch(error => {
                // Обработка ошибок
                console.error('Произошла ошибка:', error);
            });
    };
    input.click();
});