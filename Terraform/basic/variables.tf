variable "file_content" {
  description = "Content to be written in the example file."
  type        = string
  default     = "Hello, Terraform!"
}

variable "fruits" {
  description = "List of fruits."
  type        = list(string)
  default     = ["apple", "banana", "cherry"]
}

variable "person" {
  description = "A map representing a person's details."
  type        = map(string)
  default     = {
    name = "John Doe"
    age  = "30"
    city = "New York"
  }
}

variable "car_object" {
  description = "An object representing a car."
  type = object({
    make  = string
    model = string
    year  = number
  })
  default = {
    make  = "Toyota"
    model = "Corolla"
    year  = 2020
  }
}