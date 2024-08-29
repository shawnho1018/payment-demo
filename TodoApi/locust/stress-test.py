import time, string
import random
from locust import HttpUser, task, between

class QuickstartUser(HttpUser):
    wait_time = between(1, 5)
    db_entries = []

        
    @task(1)
    def get_all_orders(self):
        self.client.get("/order")

    @task(1)
    def view_order(self):
        if len(self.db_entries) > 0:
            entry = random.choice(self.db_entries)
            self.client.get(f"/order/{entry}")
    @task(1)
    def post_order(self):
        new_entry = ""
        while True:
            new_entry = f"Order{random.randint(100, 999)}"
            if new_entry not in self.db_entries:
                break

        self.db_entries.append(new_entry)
        storeID=''.join(random.choice(string.ascii_lowercase) for i in range(10))
        amount = random.randint(100, 1000)
        self.client.post("/order", json={"storeId":f"{storeID}","orderNumber":f"{new_entry}","amount":f"{amount}"})
    @task(1)
    def put_order(self):
        if len(self.db_entries) > 0:
            entry = random.choice(self.db_entries)
            self.client.put("/order", json={"orderNumber":f"{entry}","storeID":"MacDonald","amount":"500"})
    @task(1)
    def delete_order(self):
        if len(self.db_entries) > 0:
            entry = random.choice(self.db_entries)
            self.client.delete(f"/order/{entry}")
            self.db_entries.remove(entry)