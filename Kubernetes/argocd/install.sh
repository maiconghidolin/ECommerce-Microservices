#!/bin/sh

kubectl create namespace argocd

kubectl apply -n argocd -f install.yaml