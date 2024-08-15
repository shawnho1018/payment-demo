provider "google" {
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

resource "google_sql_database_instance" "default" {
  name             = "order-db"
  region           = var.region
  database_version = "POSTGRES_14"
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
