terraform {
  required_providers {
    local = {
      source = "hashicorp/local"
      version = "2.5.3"
    }

     random = {
      source = "hashicorp/random"
      version = "3.7.2"
    }
  }
}

resource "random_uuid" "my_random_uuid" {
}

resource "random_pet" "my_random_pet" {
  length    = 2
  separator = " "
}

data "local_file" "external_file" {
  filename = "datasource.txt"
}

resource "local_file" "test_file" {
  filename = "example.txt"
  content  = <<-EOF
    ${var.file_content}

    Fruits List:
    %{ for fruit in var.fruits ~}
    - ${fruit}
    %{ endfor ~}

    Person Details:
    Name: ${var.person.name}
    Age: ${var.person.age}
    City: ${var.person.city}

    Car Information:
    Make: ${var.car_object.make}
    Model: ${var.car_object.model}
    Year: ${var.car_object.year}
    
    Generated UUID: ${random_uuid.my_random_uuid.result}

    Generated Pet Name: ${random_pet.my_random_pet.id}

    External File Content: ${data.local_file.external_file.content}
  EOF
}

output "my_output" {
  value = "${random_uuid.my_random_uuid.result} - ${random_pet.my_random_pet.id}"
}