import json
import unittest
import requests


class TestAuthorization(unittest.TestCase):
    def setUp(self):
        self.base_url = "https://localhost:7276/api/auth"
        self.route = "/postauth"

    def test_post_valid_authorization(self):
        login_or_email = "k_prog_"
        password = "717171"

        response = requests.post(f"{self.base_url}{self.route}?loginOrEmail={login_or_email}&password={password}", verify=False)
        self.assertEqual(response.status_code, 200)

        feedback = response.json()
        self.assertEqual(feedback["success"], True)
        self.assertEqual(feedback["message"], "Авторизация успешна")

    def test_post_not_valid_authorization(self):
        login_or_email = "12345678"
        password = "12345678"

        response = requests.post(f"{self.base_url}{self.route}?loginOrEmail={login_or_email}&password={password}", verify=False)
        self.assertEqual(response.status_code, 400)

        feedback = response.json()
        self.assertEqual(feedback["success"], False)
        self.assertEqual(feedback["message"], "Данные неверные")

    def test_post_null_authorization(self):
        login_or_email = ""
        password = ""

        response = requests.post(f"{self.base_url}{self.route}?loginOrEmail={login_or_email}&password={password}", verify=False)
        self.assertEqual(response.status_code, 400)

        self.assertEqual(response.json()["errors"], {
            "password": ["The password field is required."],
            "loginOrEmail": ["The loginOrEmail field is required."]
        })