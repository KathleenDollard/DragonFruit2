# Required values

Required values differ from validation because it is performed on DataValues before the args object is created. This prohibits creating an invalid object and breaking the `SetsRequiredPropertiesAttribute` contract. Validation is performed on the args object for simplicity.