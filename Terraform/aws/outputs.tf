output "private_dns" {
  value = aws_instance.example_instance.private_dns
}

# output "elastic_ip" {
#   value = aws_eip.example_eip.public_ip
# }