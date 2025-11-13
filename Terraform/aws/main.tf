module "network" {
  source = "./modules/network"
  prefix = "example"
  vpc_cidr_block = "10.0.0.0/16"
  subnet_cidr_blocks = ["10.0.1.0/24", "10.0.2.0/24"] 
}

data "aws_secretsmanager_secret" "example_secret" {
    arn = "arn:aws:secretsmanager:us-west-2:217777498035:secret:dev/terraform/example-1hLTDT"
}

data "aws_secretsmanager_secret_version" "current_example_secret_version" {
    secret_id = data.aws_secretsmanager_secret.example_secret.id
}

resource "aws_instance" "example_instance" {
  ami                     = "ami-00e15f0027b9bf02b"
  instance_type           = "t3.micro"
  subnet_id               = module.network.subnet_ids[0]
  vpc_security_group_ids  = [module.network.security_group_id]
  
  user_data = <<-EOF
            #!/bin/bash
            echo "Secret Value: ${data.aws_secretsmanager_secret_version.current_example_secret_version.secret_string}" > secret.txt
            EOF

  tags = {
    Name = "ExampleInstance"
  }
}

# resource "aws_ssm_parameter" "example_parameter" {
#   name  = "vm_ip"
#   type  = "String"
#   value = aws_eip.example_eip.public_ip
# }
