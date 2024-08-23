data "google_project" "project" {
  project_id = var.project_id
}
module "project-services" {
  source  = "terraform-google-modules/project-factory/google//modules/project_services"
  project_id  = data.google_project.project.project_id
  disable_services_on_destroy = false
  activate_apis = [
    "anthos.googleapis.com",
    "vpcaccess.googleapis.com",
    "container.googleapis.com",
    "cloudscheduler.googleapis.com",
    "cloudresourcemanager.googleapis.com",
    "clouderrorreporting.googleapis.com",
    "dns.googleapis.com",
    "cloudtrace.googleapis.com",
    "compute.googleapis.com",
    "gkeconnect.googleapis.com",
    "gkehub.googleapis.com",
    "iam.googleapis.com",
    "iamcredentials.googleapis.com",
    "logging.googleapis.com",
    "meshca.googleapis.com",
    "meshconfig.googleapis.com",
    "monitoring.googleapis.com",
    "stackdriver.googleapis.com"
  ]
}
