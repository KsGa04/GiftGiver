import json
import unittest
import requests


class TestGetByIdProduct(unittest.TestCase):
    def setUp(self):
        self.base_url = "https://localhost:7276"
        self.route = "/IdProduct"

    def test_get_product_by_id(self):
        # Arrange
        product_id = 124203940
        expected_status_code = 200
        expected_product = {
            "ПодаркиId": 124203940,
            "Наименование": "Термокружка для кофе 500 мл автомобильная термос термостакан",
            "Цена": 782.0000
        }

        # Act
        response = requests.get(f"{self.base_url}{self.route}?id={product_id}", verify=False)
        product_expected = response.text
        data = json.loads(product_expected)
        # Assert
        self.assertEqual(response.status_code, expected_status_code)
        self.assertEqual(data["наименование"], expected_product["Наименование"])

    def test_get_product_not_found(self):
        # Arrange
        product_id = 999
        expected_status_code = 404

        # Act
        response = requests.get(f"{self.base_url}{self.route}?id={product_id}", verify=False)

        # Assert
        self.assertEqual(response.status_code, expected_status_code)