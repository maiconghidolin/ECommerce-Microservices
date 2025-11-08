terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
      version = "6.20.0"
    }
  }
}

provider "aws" {
  region = "us-west-2"
  profile = "default"
}

data "aws_secretsmanager_secret" "example_secret" {
    arn = "arn:aws:secretsmanager:us-west-2:217777498035:secret:dev/terraform/example-1hLTDT"
}

data "aws_secretsmanager_secret_version" "current_example_secret_version" {
    secret_id = data.aws_secretsmanager_secret.example_secret.id
}

resource "aws_vpc" "example_vpc" {
  cidr_block = "10.0.0.0/16"  
}

resource "aws_subnet" "example_subnet" {
  vpc_id            = aws_vpc.example_vpc.id
  cidr_block        = "10.0.1.0/24"
  availability_zone = "us-west-2a"
}

resource "aws_instance" "example_instance" {
  ami           = "ami-00e15f0027b9bf02b"
  instance_type = "t3.micro"
  subnet_id     = aws_subnet.example_subnet.id
  vpc_security_group_ids = [aws_security_group.example_sg.id]
  user_data     = <<-EOF
                  #!/bin/bash
                  echo "Secret Value: ${data.aws_secretsmanager_secret_version.current_example_secret_version.secret_string}" > secret.txt
                  EOF

  tags = {
    Name = "ExampleInstance"
  }
}

resource "aws_internet_gateway" "example_igw" {
  vpc_id = aws_vpc.example_vpc.id
}

resource "aws_eip" "example_eip" {
  instance = aws_instance.example_instance.id
  depends_on = [ aws_internet_gateway.example_igw ]
}

resource "aws_ssm_parameter" "example_parameter" {
  name  = "vm_ip"
  type  = "String"
  value = aws_eip.example_eip.public_ip
}

resource "aws_route_table" "example_route_table" {
  vpc_id = aws_vpc.example_vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.example_igw.id
  }
}

resource "aws_route_table_association" "example_route_table_association" {
  subnet_id      = aws_subnet.example_subnet.id
  route_table_id = aws_route_table.example_route_table.id
}

resource "aws_security_group" "example_sg" {
  name        = "allow_ssh"
  description = "Allow SSH"
  vpc_id      = aws_vpc.example_vpc.id

  tags = {
    Name = "AllowSSH"
  }
}

resource "aws_vpc_security_group_ingress_rule" "example_sg_ingress_rule" {
  security_group_id = aws_security_group.example_sg.id
  from_port        = 22
  to_port          = 22
  ip_protocol      = "tcp"
  cidr_ipv4        = "0.0.0.0/0"
}

resource "aws_vpc_security_group_egress_rule" "example_sg_egress_rule" {
  security_group_id = aws_security_group.example_sg.id
  ip_protocol      = "-1"
  cidr_ipv4        = "0.0.0.0/0"
}

output "private_dns" {
  value = aws_instance.example_instance.private_dns
}

output "elastic_ip" {
  value = aws_eip.example_eip.public_ip
}