import json
import unittest
import requests


class TestProductsController(unittest.TestCase):
    def setUp(self):
        self.base_url = "https://localhost:7276"
        self.route = "/allproducts"

    def test_get_all_products_by_id(self):
        user_id = 1

        response = requests.get(f"{self.base_url}{self.route}/{user_id}", verify=False)

        self.assertEqual(response.status_code, 200)

        products = response.json()
        self.assertIsInstance(products, list)

        self.assertTrue(len(products) > 0)

        for product in products:
            self.assertIsInstance(product, dict)
            self.assertIn("подаркиId", product)
            self.assertIn("наименование", product)
