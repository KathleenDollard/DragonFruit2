# Transforms

_Future enhancement_

If we provide a mechanism for transforms, they would occur between checking for required and validation.

Ceiling, floor and adjacent integer concepts are one example, but the easiest to visual may be string transforms, such as trimming, changing case, adding a prefix/suffix if not present, shortening, padding, formatting (such as a phone number), etc.

The concept may be named as "force", such as `ForceUpper`.

Transforms would be managed in the same way as validation: there would be a `Register` method, optionally attributes that lead to the generation of the Register method, and a callback.