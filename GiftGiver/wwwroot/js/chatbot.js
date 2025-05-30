﻿document.getElementById("more-btn").style.display = "none";
document.querySelector(".chat-input input").setAttribute("disabled", "disabled");
document.getElementById("send").setAttribute("disabled", "disabled");
document.getElementById("start-btn").addEventListener("click", function (event) {
    event.preventDefault();
    this.style.display = "none";
    document.querySelector(".chat-input input").removeAttribute("disabled");
    document.getElementById("send").removeAttribute("disabled");

    var chatMessages = document.querySelector(".chat-messages");
    var newMyMessage = createMessageElement("message", questions[currentQuestionIndex]);
    chatMessages.appendChild(newMyMessage);
    currentQuestionIndex++;
});


function createMessageElement(className, content, isTheir = false) {
    var messageElement = document.createElement("div");
    messageElement.className = "message " + className;
    messageElement.innerHTML = `<div class='avatar ${isTheir ? "their-avatar" : ""}'></div><div class='message-content ${isTheir ? "their" : ""}'>${content}</div>`;
    return messageElement;
}

function scrollToBottom(element) {
    element.scrollTop = element.scrollHeight;
}
var botDictionary = {
    "Какой категории вам необходим подарок?": "",
    "Кому хотите подарить подарок?": "",
    "Какого возраста получатель?": ""
};
var currentQuestionIndex = 0;
var questions = Object.keys(botDictionary);
document.getElementById("send").addEventListener("click", function (event) {
    event.preventDefault();
    sendMessage();
});
document.querySelector(".chat-input input").addEventListener("keypress", function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        sendMessage();
    }
});
var totalDisplayedGifts = 0;

function getGiftsFromServer() {
    var chatMessages = document.querySelector(".chat-messages");
    $.ajax({
        type: "POST",
        url: "/GetGifts",
        contentType: "application/json",
        data: JSON.stringify(botDictionary),
        success: function (data) {

            if (data.totalCount === 0) {
                var userMessage = "Похожих подарков нет. Перезагрузите страницу чтобы начать заново.";
                var newTheirMessage = createMessageElement("message", userMessage, false);
                chatMessages.appendChild(newTheirMessage);
            }
            else {
                var totalCount = data.totalCount;
                if (totalCount != 0) {
                    var textBlock = document.createElement("div");
                    textBlock.className = "message";

                    var avatar = document.createElement("div");
                    avatar.classList.add("avatar");
                    textBlock.appendChild(avatar);

                    var textBlockContentElement = document.createElement("div");
                    textBlockContentElement.classList.add("message-content", "gift");
                    textBlockContentElement.textContent = "Вот что мне удалось подобрать";
                    textBlock.appendChild(textBlockContentElement);
                    chatMessages.appendChild(textBlock);


                    var giftBlock = document.createElement("div");
                    giftBlock.className = "message";

                    var avatarElement = document.createElement("div");
                    avatarElement.classList.add("avatar");
                    giftBlock.appendChild(avatarElement);

                    var messageContentElement = document.createElement("div");
                    messageContentElement.classList.add("message-content", "gift");
                    giftBlock.appendChild(messageContentElement);

                    var gifts = data.gifts;

                    gifts.forEach(function (gift) {
                        var giftColumn = document.createElement("div");
                        giftColumn.classList.add("gift-column");

                        var imageElement = document.createElement("img");
                        imageElement.src = 'data:image/png;base64,' + gift.image;
                        giftColumn.appendChild(imageElement);

                        var nameElement = document.createElement("p");
                        nameElement.textContent = gift.name;
                        giftColumn.appendChild(nameElement);

                        var nameElement = document.createElement("p");
                        nameElement.classList.add("count");
                        nameElement.textContent = gift.count + " ₽";
                        giftColumn.appendChild(nameElement);

                        var addElement = document.createElement("button");
                        addElement.classList.add("add-wish");
                        addElement.id = "add-wish";
                        addElement.textContent = "Добавить";
                        addElement.dataset.giftId = gift.id;
                        addElement.addEventListener('click', handleAddWishClick);
                        giftColumn.appendChild(addElement);

                        var linkElement = document.createElement("a");
                        linkElement.classList.add("gift-link");
                        linkElement.href = gift.link;
                        linkElement.textContent = "Подробнее";
                        giftColumn.appendChild(linkElement);

                        messageContentElement.appendChild(giftColumn);
                        totalDisplayedGifts++;
                        document.getElementById("more-btn").style.display = "block";
                    });
                    chatMessages.appendChild(giftBlock);
                }
                else {
                    var userMessage = "Похожие подарки закончились";
                    var newTheirMessage = createMessageElement("message", userMessage, false);
                    chatMessages.appendChild(newTheirMessage);
                    document.getElementById("more-btn").style.display = "none";
                    return;
                }
            }
            scrollToBottom(chatMessages);
            document.getElementById("more-btn").style.display = "block";
        }
    });
}

function sendMessage() {
    var chatMessages = document.querySelector(".chat-messages");
    var inputField = document.querySelector("input[name='text']");
    var userMessage = inputField.value;
    var newTheirMessage = createMessageElement("their-message", userMessage, true);
    chatMessages.appendChild(newTheirMessage);
    scrollToBottom(chatMessages);

    if (currentQuestionIndex === questions.length && !/^\d+$/.test(userMessage)) {
        var errorMessage = createMessageElement("message", "Ошибка: введите только цифры.");
        chatMessages.appendChild(errorMessage);
        scrollToBottom(chatMessages);
        return;
    }
    if (currentQuestionIndex > 0) {
        botDictionary[questions[currentQuestionIndex - 1]] = userMessage;

        // Проверка категории
        if (currentQuestionIndex === 1 && !["Игрушка", "Электроника", "Хобби", "Бижутерия", "Набор", "Косметика", "Игры", "Кухня", "Путешествия", "Декор", "Аниме"].includes(userMessage)) {
            var errorMessage = createMessageElement("message", "Необходима категория: Игрушка, Электроника, Хобби, Бижутерия, Набор, Косметика, Игры, Кухня, Путешествия, Декор, Аниме");
            chatMessages.appendChild(errorMessage);
            scrollToBottom(chatMessages);
            return;
        }
        if (currentQuestionIndex === 2 && !["Любой", "Мужчина", "Женщина", "Парень", "Девушка", "Ребенок"].includes(userMessage)) {
            var errorMessage = createMessageElement("message", "Необходим получатель: Мужчина, Женщина, Парень, Девушка, Ребенок");
            chatMessages.appendChild(errorMessage);
            scrollToBottom(chatMessages);
            return;
        }
    }

    if (currentQuestionIndex > 0) {
        botDictionary[questions[currentQuestionIndex - 1]] = userMessage;
    }
    if (currentQuestionIndex >= questions.length) {
        getGiftsFromServer();
    } else {
        var newMyMessage = createMessageElement("message", questions[currentQuestionIndex]);
        chatMessages.appendChild(newMyMessage);
        scrollToBottom(chatMessages);
        currentQuestionIndex++;
    }

    inputField.value = "";
}

document.getElementById("more-btn").addEventListener("click", function (e) {
    e.preventDefault();
    getGiftsFromServer();
});

function handleAddWishClick(event) {
    event.preventDefault();
    var giftId = event.target.dataset.giftId;
    fetch(`/Home/ДобавитьЖелаемое?подарокId=${giftId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Ошибка добавления в желаемое');
            }
        })
        .catch(error => {
            console.error(error);
        });
}