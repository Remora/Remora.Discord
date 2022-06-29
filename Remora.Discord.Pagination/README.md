Remora.Discord.Pagination
=========================

This package provides a simple implementation of pagination for messages via 
message component buttons, using the interactivity framework provided via 
`Remora.Discord.Interactivity`.

## Structure
This library uses the in-memory model to persist pagination data (such as the
available pages, the current page, and the message owner) and allow the user to
flip through one or more "pages" of content via interaction buttons.

## Usage
You may create paginated messages by calling one of the following extension
methods on `FeedbackService`.

```c#
FeedbackService.SendPaginatedMessageAsync(channelID, sourceUserID, pages);
FeedbackService.SendContextualPaginatedMessageAsync(sourceUserID, pages);
FeedbackService.SendPrivatePaginatedMessageAsync(user, pages);
```

The pages are provided as a list of embed objects, which can be created either 
manually or via one of the available factory methods on the `PageFactory` or 
`PaginatedEmbedFactory` classes.

```c#
var page = PageFactory.FromFields(fields);
var pages = PaginatedEmbedFactory.PagesFromCollection(items, item => CreateEmbedFromItem(item));
var pages = PaginatedEmbedFactory.SimpleFieldsFromCollection(items, item => item.FieldTitle, item => item.FieldValue);
```

You can also tweak the appearance of the buttons and help text by way of 
`PaginatedAppearanceOptions`, which can be passed along when sending a paginated
message.
