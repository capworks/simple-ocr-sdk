# simple-ocr-sdk

## About
Simple OCR SDK is intended to make it easy to integrate OCR usage into you application without any prior knowlegde about OCR, image preparations or the OCR service providers special quirks.

Each OCR provider is a bit different, and like to sort their outputs in their own unique way. When you are past integrating to the API, it's always a hurdle learning the new model and parsing it into something you can use for you futher processing og just plain text. This make the efford of testing an OCR provider to match your needs very time comsuming.

Hopefully this SDK can help you skip the boring stuff and get straight to developing your application.
The SDK supports Azure Vision API, Google Vision API for images and PdfBox for pdfs.

### How does it work?
The SDK handles the EXIF orientation, and compression if nessesary (only images).
The API result will be transformed into a metadata result model with relative coordinates, that will make it easy for you to highlight your findings in your UI.

### Limitations
The current SDK support png, jpeg, jpg and PDF (only 'readable' PDFs).

### PPFs
The vision API does not support PDF's where the content is an image (if you open the pdf manually and cannot select text with the cursor). If it's an image PDF or a unsupported PDF type, then the PDF needs to be converted into an image and run throught a OCR engine to extract the text.

If the given PDF is not readble, the OcrResult.Error will contain a PdfNotReadableException.

This SDK does not provide the means to convert from PDF to image, due to licenses limitations. If you are building an open source solution GhostScript is the most popular choice. For commercial use a less costly alternative is Docotic.Pdf. We have had very good experiences with them, here is an example of pdf-to-image: <a href ="https://bitmiracle.com/pdf-library/help/extract-images.aspx" target ="_blank">https://bitmiracle.com/pdf-library/help/extract-images.aspx </a>.


## Example
There is a demo app demonstrating how to get a OCR result using Google's vision API.
https://github.com/DineroRegnskab/simple-ocr-sdk/tree/master/SimpleOcrSDK/Demo


## How to init the engine
Google and Azure' OCR requires subscriptions, but the pdf engine is free to use.
For the image OCR you can freely choose whatever engine you prefer, the result might differ depending on engine and image quality.

### Google Vision API
```cs
var apiKey = "[Your-google-vision-api-key]";
var applicationName = "[Name-of-of-your-application-for-your-own-tracking-in-google]";

var ocr = GoogleOcrEngine.Build(new GoogleOcrConfigurations(apiKey, applicationName));
```

### Azure Vision API
```cs
var subcriptionKey = "[Your-azure-subscription-key]";

var ocr = AzureOcrEngine.Build(new AzureConfigurations(subcriptionKey));
```

### PdfBox 
```cs
var pdfEngine = PdfOcrEngine.Build()

```

## Usage

### How to OCR a file
You can do this with the Google, Azure or Pdf engine. The following example is with google, but the approach is the sa

#### Image

From File or Stream.
```cs
string fullPathToImage = "C:\Ocr-images\my-test-img.png";
ImageFormatEnum imageFormat = ImageFormatEnum.Png; //Depends on your image

// GOOGLE
OcrResult googleResult = await googleOcr.OcrImage(filePath, imageFormat);

// AZURE
OcrResult azureResult = await azureOcr.OcrImage(filePath, imageFormat);
string textInFile = azureResult.TextFound()? azureResult.FormattedResult.GetPlainTextWithLineBreaks() : "";
```

#### PDF
From file or byte array.
```cs
string fullPathToImage = "C:\Ocr-images\my-test.pdf";
ImageFormatEnum imageFormat = ImageFormatEnum.Pdf; 

OcrResult imageContent = await pdfOcr.OcrPdf(filePath, imageFormat);
string textInFile = imageContent.TextFound()? imageContent.FormattedResult.GetPlainTextWithLineBreaks() : "";
```

### How to get data
```cs
OcrResult ocrResult; //from whatever engine you prefer
string textInFile = ocrResult.TextFound()? ocrResult.FormattedResult.GetPlainTextWithLineBreaks() : "";
```

### Error handling
```cs
private static async Task PerformAction(OcrResult result)
{
    if (result.HasError)
    {
        if (result.Error is FileNotFoundException)
        {
            //Check you file path
        }
        else if(result.Error is PdfNotReadableException)
        {
            //Convert pdf to image and do Azure or Google ocr
        }
        else if(result.Error is ImageProportionsToSmallException)
        {
            //The Visions API need the image to be above a certain size before it makes sense to process
            //For Azure the min. size is 50x50 pixels
        }
        else if(result.Error is ImageProportionsToLargeException)
        {
            //Down scale the image
        }
        else
        {
            //Handle exception (IO, Network, API exception ect.)
        }
    }
    
    if(!result.TextFound)
    {
        // Either the vision api could not find anything or what seemed like a readable pdf, 
        // didn't contain any text
    }
}
```

## Result model explained
Depending on the type of OCR you will get a different implementation of OcrResult: GoogleOcrResult, AzureOcrResult eller PdfOcrResult.
Azure and Google's results contain a 'RAW' model with the original data returned from the APIs. You only need to access the RAW results in case you need other data then mapped in the metadata model or get API specific properties.

```cs   
public abstract class OcrResult
{
    public TimeSpan ProcessTime { get; }
    public ImageContent Content { get; }
    public Exception Error { get; }
    bool HasError { get; }
    bool TextFound { get; }
}
```


### RawResult
As the name says ```RawResult``` is the raw Google Vision API result. It contains all the result data received from the API.

### FormattedResult
The result is formatted into Lines, Sentences and Words. Words and sentences contains relative coordinates.

```cs
public class ImageContent
{
    /// <summary>
    /// All sentences found on the image
    /// </summary>
    public List<Sentence> Sentences { get; }

    /// <summary>
    /// All words found on the image.
    /// </summary>
    public List<Word> Words { get;  }

    /// <summary>
    /// All lines found on the image. 
    /// Dictionary with zero index line numbers as key, and list of sentences as value.
    /// </summary>
    public Dictionary<int, List<Sentence>> Lines { get; set; }
```

A Line consist of a number of one or more senteces and a sentece consist of one or more words.

Example of Words:

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/receipt%20explained%20words.jpg "Words")


Example of Senteces:

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/receipt%20explained%20sentences.jpg "Sentences")


Example of Lines:

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/receipt%20explained%20lines.jpg "Lines")


All of it together:

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/receipt%20explained%20-%20all.jpg "Words, Sentences and Lines")


#### Coordinates
```
public class Coordinate
{
    public double X { get; }
    public double Y { get; }
    public double Height { get; }
    public double Width { get; }
}
```

Words and sentences has relative coordinates. The x and y coordinates are measured from the top-left corner of the image. Meaning the fist pixel of the image is (0,0) and the last pixel is right-bottom corner (100,100).

X and y, represent the top left corner of the word or sentence.

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/coordinates.png "Coordinates")


## License

MIT © [dineroregnskab](mailto:info@dinero.dk)
