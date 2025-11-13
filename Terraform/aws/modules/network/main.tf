resource "aws_vpc" "vpc" {
  cidr_block = var.vpc_cidr_block

  tags = {
    Name = "${var.prefix}-vpc"
  }
}

data "aws_availability_zones" "availability_zone" {
  state = "available"
}

resource "aws_subnet" "subnets" {
  count             = length(var.subnet_cidr_blocks)
  vpc_id            = aws_vpc.vpc.id
  cidr_block        = var.subnet_cidr_blocks[count.index]
  availability_zone = data.aws_availability_zones.availability_zone.names[count.index % length(data.aws_availability_zones.availability_zone.names)]

  tags = {
    Name = "${var.prefix}-subnet-${count.index}"
  }
}


resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.vpc.id
}

# resource "aws_eip" "eip" {
#   instance = aws_instance.instance.id
#   depends_on = [ aws_internet_gateway.igw ]
# }

resource "aws_route_table" "route_table" {
  vpc_id = aws_vpc.vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.igw.id
  }
}

resource "aws_route_table_association" "route_table_association" {
  count          = length(var.subnet_cidr_blocks)
  subnet_id      = aws_subnet.subnets[count.index].id
  route_table_id = aws_route_table.route_table.id
}

resource "aws_security_group" "sg" {
  name        = "${var.prefix}-allow-ssh"
  vpc_id      = aws_vpc.vpc.id

  tags = {
    Name = "${var.prefix}-allow-ssh"
  }
}

resource "aws_vpc_security_group_ingress_rule" "sg_ingress_rule" {
  security_group_id = aws_security_group.sg.id
  from_port         = 22
  to_port           = 22
  ip_protocol       = "tcp"
  cidr_ipv4         = "0.0.0.0/0"
}

resource "aws_vpc_security_group_egress_rule" "sg_egress_rule" {
  security_group_id = aws_security_group.sg.id
  ip_protocol       = "-1"
  cidr_ipv4         = "0.0.0.0/0"
}