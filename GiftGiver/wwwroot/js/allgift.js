﻿document.querySelectorAll('.wish-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        var подарокId = this.getAttribute('data-подарокid');
        fetch('/Home/ДобавитьЖелаемое?подарокId=' + подарокId, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (!data.success) {
                    if (data.redirectToAuthorization) {
                        window.location.href = '/Home/Authorization';
                    } else {
                        console.error('Ошибка добавления в желаемое');
                    }
                } else {
                }
            })
            .catch(error => {
                console.error(error);
            });
    });
});

    //document.addEventListener("DOMContentLoaded", function (event) {
    //    var images = document.querySelectorAll('.grid-item-allproducts img');
    //    images.forEach(function (image) {
    //        var timeoutId;
    //        image.addEventListener("mouseover", function (e) {
    //            timeoutId = setTimeout(function () {
    //                var parentDiv = e.target.parentNode;
    //                var giftName = parentDiv.querySelector('.grid-cell-name').innerText;
    //              /*  var giftPrice = parentDiv.querySelector('.grid-cell-price').innerText;*/ // Предположим, что у вас есть отдельный элемент для цены подарка
    //                var message = 'Подарок: ' + giftName /*+ ', Цена: ' + giftPrice*/;
    //                alert(message);
    //            }, 1000); // Задержка в 3 секунды
    //        });
    //        image.addEventListener("mouseout", function () {
    //            clearTimeout(timeoutId); // Очищаем задержку при уходе курсора с изображения
    //        });
    //    });
    //});

    //document.addEventListener("DOMContentLoaded", function (event) {
    //    var images = document.querySelectorAll('.grid-item-allproducts img');
    //    images.forEach(function (image) {
    //        var overlay = document.createElement('div');
    //        overlay.classList.add('image-overlay');
    //        image.parentNode.appendChild(overlay);

    //        image.addEventListener('mouseover', function () {
    //            overlay.style.display = 'block';
    //            // Тут можно также показать кнопку "Добавить"
    //            var addButton = image.parentNode.querySelector('.wish-btn');
    //            addButton.style.display = 'block';
    //        });

    //        image.addEventListener('mouseout', function () {
    //            overlay.style.display = 'none';
    //            // Тут можно также скрыть кнопку "Добавить"
    //            var addButton = image.parentNode.querySelector('.wish-btn');
    //            addButton.style.display = 'none';
    //        });
    //    });
    //});
    //$(document).ready(function () {
    //    $('#search-button').click(function () {
    //        $.ajax({
    //            url: '/Home/Find', // замените "Home" на имя вашего контроллера
    //            type: 'GET', // или 'POST', в зависимости от метода Find
    //            success: function (result) {
    //                // обработка результата
    //            },
    //            error: function () {
    //                // обработка ошибки
    //            }
    //        });
    //    });
    //});