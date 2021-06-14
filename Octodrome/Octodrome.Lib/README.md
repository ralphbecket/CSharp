# Octodrome
Ralph Becket
This document started 20201-02-19

## Goal

My wife started her own small business and I helped by setting up her IT
system.  Rather than buy an off the shelf system and spend much time learning
how to use it, how to tailor it, and ultimately how to adapt to that system's
assumptions regarding how a business should operate, I suggested to her that
she simply use the _Office 365_ package that she was already paying for:
- _Outlook_ for e-mail and contacts;
- _Word_ for official documents;
- _Excel_ for tracking time, accounting, and so forth;
- _OneNote_ as an ad hoc customer and project database.
I wrote a fairly simple C# program to traverse her employees' time-sheet 
spreadsheets to produce an overall per-employee and per-client spreadsheet
for any given interval (the main uses being billing and reporting).

This all worked splendidly, so much so that when the idea was mooted that the
ad hoc system be replaced with a professional system, everybody (who had
experience with such things) said they felt they were much more productive
with the current scheme.  In particular, people found that it was helpful to
be able to "shelve" an update half-way through for completion later rather
than to have an incomplete update rejected for violating business rules.  Of
course, I provided my wife with the tools to find all those places where
business rules were broken (e.g., wrong names, missing time sheet data, etc.).

The conclusion I have drawn from this is that for small businesses,
particularly new businesses, it is more valuable that a system be flexible
and _simple_ than that it be formal and comprehensive.

Octodrome is my effort to provide a simple, integrated, extensible, IT
system suitable for new businesses.

## The Database

The database records the state of the business: contacts, customers, staff,
time sheets, project notes, and so forth.  Some requirements:
- it must be flexible -- users should be free to arrange things to suit the
business;
- it must be possible for multiple people to edit the database at the same
time;
- data must never be lost: every change should be logged, providing for
recovery, roll-back, and audit trails (i.e., who changed what and when);
- it should scale to support any reasonably sized small business (e.g., 
at least ten staff, with hundreds to thousands of clients);
- consistency "rules" should not be enforced, although it should make it
simple to identify all places in the database where such rules are violated
(e.g., "every client should have a primary contact phone number").

### The Database Structure

The database is a graph structure, including the following kinds of vertices:
- strings and text objects, dates, times, numbers, booleans, etc.;
- references to other vertices;
- ordered lists;
- dictionaries (i.e., mappings from names to vertices).
Others may be necessary, I will have to see.

Each vertex has a unique identity.

The database records the entire history of every vertex: data is never
deleted.

The edit history for each vertex is a sequence of before/after changes, each
annotated with a time stamp and the identity of the editor.  The latest state
of the database can be recovered from the complete edit history and changes
can be rolled back if necessary.




## Code Structure

Since the items in the graph are of different types, they will have different
kinds of values, different editing functions, different presentation 
functions, and so forth.  This doesn't lend itself greatly to OO style.

I have this idea that a user will carry out some editing in one bulk operation
and then reconcile those edits with the database (the authoritative current
state).

I think, therefore, that each item edit need only record the change from its
initial state to its final state (i.e., the user may edit the same field many
times before reconciling with the database).

In this sense an edit history is only needed with respect to the initial
states.

To that end, reconciliation only needs the list of changes that have been 
made to those items that have been edited (and we include creation as part
of the editing process).

So what is this going to look like from the client side?

The client downloads some items.

Let's call this a document view: a mapping from IDs to items.

The edits need only record the changes between the loaded DB versions
and the state at the point of reconciliation (i.e., DB update).

Hmm, maybe the document should just contain a set of before and (where edited)
after items.

No, we need an undo/redo buffer.


