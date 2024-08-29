provider "google" {
  project = var.project_id
  region = var.region
}
provider "google-beta" {
  project = var.project_id
  region = var.region
}
#resource "google_sql_database_instance" "read_replica" {
#  name                 = "order-db-replica"
#  master_instance_name = google_sql_database_instance.default.name
#  region               = "asia-southeast1"
#  database_version     = "POSTGRES_14"

#  settings {
#    tier              = "db-f1-micro"
#    availability_type = "ZONAL"
#    disk_size         = "100"
#  }
  # set `deletion_protection` to true, will ensure that one cannot accidentally delete this instance by
  # use of Terraform whereas `deletion_protection_enabled` flag protects this instance at the GCP level.
#  deletion_protection = false
#}

data "google_kms_key_ring" "keyring" {
  name     = "gcs-keyring"
  location = var.region
}

resource "google_kms_crypto_key" "key" {
  name            = "orderdb4"
  key_ring        = data.google_kms_key_ring.keyring.id
  rotation_period = "7776000s"

  lifecycle {
    prevent_destroy = false
  }
}

resource "google_project_service_identity" "gcp_sa_cloud_sql" {
  provider = google-beta
  service  = "sqladmin.googleapis.com"
}

resource "google_kms_crypto_key_iam_binding" "crypto_key" {
  provider      = google-beta
  crypto_key_id = google_kms_crypto_key.key.id
  role          = "roles/cloudkms.cryptoKeyEncrypterDecrypter"

  members = [
    "serviceAccount:${google_project_service_identity.gcp_sa_cloud_sql.email}",
  ]
}

resource "google_sql_database_instance" "default" {
  name             = "order-db"
  region           = var.region
  database_version = "POSTGRES_14"
  encryption_key_name = google_kms_crypto_key.key.id
  settings {
    tier = "db-f1-micro"
    ip_configuration {
      psc_config {
        psc_enabled = true
        allowed_consumer_projects = [var.project_id]
      }
      ipv4_enabled = false
    }
    backup_configuration {
      enabled = true
    }
    availability_type = "REGIONAL"
  }
  # set `deletion_protection` to true, will ensure that one cannot accidentally delete this instance by
  # use of Terraform whereas `deletion_protection_enabled` flag protects this instance at the GCP level.
  deletion_protection = false
}


resource "google_compute_address" "default" {
  name         = "psc-compute-address"
  region       = var.region 
  address_type = "INTERNAL"
  subnetwork   = "default"     # Replace value with the name of the subnet here.
}

resource "google_compute_forwarding_rule" "default" {
  name                  = "psc-forwarding-rule-${google_sql_database_instance.default.name}"
  region                = var.region
  network               = "default"
  ip_address            = google_compute_address.default.self_link
  load_balancing_scheme = ""
  target                = google_sql_database_instance.default.psc_service_attachment_link
}

resource "google_sql_database" "database" {
  name     = "mydb"
  instance = google_sql_database_instance.default.name
}


resource "google_sql_user" "user" {
  name     = "user"
  instance = google_sql_database_instance.default.name
  password = "password"
}
