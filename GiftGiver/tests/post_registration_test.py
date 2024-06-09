import unittest
import json
import requests
from unittest.mock import patch

class TestRegController(unittest.TestCase):
    def setUp(self):
        self.base_url = "https://localhost:7276/api/reg"
        self.route = "/postreg"

    def test_post_reg_not_valid(self):
        login = "Логин"
        email = "E-mail"
        password = "1111"
        fio = "ФИО"
        # Act
        response = requests.post(
            f"{self.base_url}{self.route}?login={login}&email={email}&password={password}&fio={fio}",
            verify=False)

        # Assert
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()["success"], False)
        self.assertEqual(response.json()["message"], "Данные невалидны")

    def test_post_reg_null(self):
        login = ""
        email = ""
        password = ""
        fio = ""
        # Act
        response = requests.post(
            f"{self.base_url}{self.route}?login={login}&email={email}&password={password}&fio={fio}",
            verify=False)
        feedback = response.json()
        # Assert
        self.assertEqual(response.status_code, 400)
        self.assertEqual(response.json()["errors"], {
            'fio': ['The fio field is required.'],
            'email': ['The email field is required.'],
            'login': ['The login field is required.'],
            'password': ['The password field is required.']
        })

    def test_post_reg_available_user(self):
        login = "k_prog_"
        email = "info@2580088.ru"
        password = "717171"
        fio = "Гаранчева Ксения"
        # Act
        response = requests.post(
            f"{self.base_url}{self.route}?login={login}&email={email}&password={password}&fio={fio}",
            verify=False)
        feedback = response.json()
        # Assert
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()["success"], False)
        self.assertEqual(response.json()["message"], "Пользователь с данным логином или email уже зарегистрирован")
