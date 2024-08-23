resource "google_bigquery_dataset" "gke_consumption" {
  dataset_id                  = "gke_cluster_consumption"
  friendly_name               = "gke_consumption1"
  description                 = "consumption dataset for GKE"
  location                    = var.region
  default_table_expiration_ms = 3600000

  labels = {
    env = "default"
  }

  access {
    role          = "OWNER"
    user_by_email = data.google_service_account.bqowner.email
  }

  access {
    role   = "READER"
    domain = "hashicorp.com"
  }
}

data "google_service_account" "bqowner" {
  account_id = "bqowner"
}
