#!/bin/bash
###
# Copyright 2022 Google LLC
#
# This function provides an easy way to add workload identity
# to a service account.
# Usage:
#   workload_identity <GSA> <PROJECT_ID> <K8S_NAMESPACE> <KSA>
#
# It also provides an easy way to validate the workload identity setup
# Usage:
#   validate_workload_identity <K8S_NAMESPACE> <KSA>
#
###
function workload_identity() {
    if [ ! $# -eq 4 ]; then
      echo "Need 4 variables. In sequence: GSA, PROJECT_ID, K8S_NAMESPACE, KSA"
      return 1
    fi
    GSA=$1
    PROJECT_ID=$2
    K8S_NS=$3
    KSA=$4
    gcloud iam service-accounts add-iam-policy-binding ${GSA}@${PROJECT_ID}.iam.gserviceaccount.com \
    --role roles/iam.workloadIdentityUser \
    --member "serviceAccount:${PROJECT_ID}.svc.id.goog[${K8S_NS}/${KSA}]"

    kubectl annotate serviceaccount ${KSA} \
    --namespace ${K8S_NS} \
    iam.gke.io/gcp-service-account=${GSA}@${PROJECT_ID}.iam.gserviceaccount.com
}

function validate_workload_identity() {
    if [ ! $# -eq 2 ]; then
      echo "Need 2 variables. In sequence: K8S_NAMESPACE, KSA"
      return 1
    fi
    K8S_NS=$1
    KSA=$2
    # Verify WS Setup
    echo "Generate a workload-identity-test pod on ${K8S_NS} space"
cat << EOF | kubectl apply -f -
apiVersion: v1
kind: Pod
metadata:
    name: workload-identity-test
    namespace: ${K8S_NS}
    labels:
        app: wit
spec:
    containers:
    - image: google/cloud-sdk:slim
      name: workload-identity-test
      command: ["sleep","infinity"]
    serviceAccountName: ${KSA}
    nodeSelector:
        iam.gke.io/gke-metadata-server-enabled: "true"
EOF
    kubectl wait --for=condition=ready pod -l app=wit -n ${K8S_NS}
    kubectl exec -it workload-identity-test \
    --namespace ${K8S_NS} \
    -- curl -H "Metadata-Flavor: Google" http://169.254.169.254/computeMetadata/v1/instance/service-accounts/default/email
    echo
    kubectl delete pod workload-identity-test -n ${K8S_NS}
}
