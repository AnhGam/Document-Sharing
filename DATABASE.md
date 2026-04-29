# Database Schema (SQLite)

The application uses a local SQLite database named `document_sharing.db`.

## Tables

### 1. tai_lieu (Documents)
Stores the main document information.
- `id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `ten`: TEXT NOT NULL (Document Name)
- `mon_hoc`: TEXT (Subject)
- `loai`: TEXT (Type)
- `duong_dan`: TEXT (File Path)
- `ghi_chu`: TEXT (Notes)
- `ngay_them`: DATETIME DEFAULT (datetime('now', 'localtime')) (Date Added)
- `kich_thuoc`: REAL (File Size in MB)
- `tac_gia`: TEXT (Author)
- `quan_trong`: INTEGER DEFAULT 0 (Is Important flag)
- `tags`: TEXT (Semicolon separated tags)
- `deadline`: DATETIME (Optional deadline)
- `is_deleted`: INTEGER DEFAULT 0 (Soft delete flag)
- `deleted_at`: DATETIME (Timestamp when soft-deleted)

### 2. categories
Stores subject and document type categories.
- `id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `name`: TEXT NOT NULL
- `type`: TEXT (subject / document_type)

### 3. collections
Stores user-defined collections.
- `id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `name`: TEXT NOT NULL
- `description`: TEXT
- `created_at`: DATETIME DEFAULT CURRENT_TIMESTAMP

### 4. collection_items
Links documents to collections (Many-to-Many).
- `id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `collection_id`: INTEGER NOT NULL (Foreign Key)
- `document_id`: INTEGER NOT NULL (Foreign Key)
- `added_at`: DATETIME DEFAULT CURRENT_TIMESTAMP
- UNIQUE(`collection_id`, `document_id`)

### 5. personal_notes
Stores private notes for documents.
- `id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `document_id`: INTEGER NOT NULL (Foreign Key)
- `content`: TEXT
- `created_at`: DATETIME DEFAULT CURRENT_TIMESTAMP
- `updated_at`: DATETIME DEFAULT CURRENT_TIMESTAMP

### 6. recent_files
Tracks recently opened documents (max 20 entries).
- `id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `document_id`: INTEGER NOT NULL UNIQUE (Foreign Key)
- `opened_at`: DATETIME DEFAULT CURRENT_TIMESTAMP

### 7. document_relations
Stores relationships between related documents (bidirectional).
- `id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `doc_id_1`: INTEGER NOT NULL (Foreign Key, always the smaller ID)
- `doc_id_2`: INTEGER NOT NULL (Foreign Key, always the larger ID)
- `relation_type`: TEXT DEFAULT 'related'
- `created_at`: DATETIME DEFAULT CURRENT_TIMESTAMP
- UNIQUE(`doc_id_1`, `doc_id_2`)

## Indexes
- `idx_tai_lieu_mon_hoc`
- `idx_tai_lieu_loai`
- `idx_tai_lieu_ngay_them`
- `idx_tai_lieu_deadline`
- `idx_collection_items_collection`
- `idx_collection_items_document`

## Recycle Bin (Soft Delete)

Documents are not permanently deleted by default. Instead:
- **Delete**: Sets `is_deleted = 1` and `deleted_at = CURRENT_TIMESTAMP`.
- **Restore**: Sets `is_deleted = 0` and `deleted_at = NULL`.
- **Permanent Delete**: `DELETE FROM tai_lieu WHERE id = @id` (removes DB record only, file on disk is untouched).
- **Empty Bin**: `DELETE FROM tai_lieu WHERE is_deleted = 1`.

All query methods filter by `is_deleted = 0` to hide soft-deleted documents from the main view.

## Recent Files

- When a document is opened, its entry is inserted/updated in `recent_files` with `opened_at = NOW`.
- Only the 20 most recent entries are kept (older entries are automatically trimmed).
- Deleted documents (`is_deleted = 1`) are excluded from the recent files list via JOIN filter.

## Document Relations

- Relations are bidirectional: `doc_id_1` is always the smaller ID, `doc_id_2` is the larger.
- Query uses `CASE WHEN` to return the related document regardless of which side it's on.
- Relations are automatically removed when either document is permanently deleted (ON DELETE CASCADE).

## Backup & Restore

- **Backup**: Copies `document_sharing.db` to a user-specified path.
- **Restore**: Copies a backup `.db` file back to the application data directory, replacing the current database.
