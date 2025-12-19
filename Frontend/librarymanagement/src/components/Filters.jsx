import { Input, Select } from "antd"
import { useEffect, useState } from "react"
import { bookStatusEnum } from "../staticStructures/bookStatus";
import "../design/Filters.scss"

export default function Filters({filterData}) {

    const [mounted, setMounted] = useState(false);
    const [filters, setFilters] = useState({
        title: null,
        author: null,
        genre: null,
        readingStatus: null
    });


    useEffect(() => {
        if (!mounted) {
            setMounted(true);
            return;
        }
        const debounce = setTimeout(() => {
            filterData(filters);
        }, 500);

        return () => clearTimeout(debounce);
    }, [filters]);

    return (
        <div className="filters-container">
            <div className="filter-item">
                <Input.Search
                    placeholder="Search by title"
                    onChange={(e) => setFilters((prev) => ({ ...prev, title: e.target.value !== '' ? e.target.value : null }))}
                    value={filters.title}
                    allowClear
                />
            </div>
            <div className="filter-item">
                <Input.Search
                    placeholder="Search by author"
                    onChange={(e) => setFilters((prev) => ({ ...prev, author: e.target.value !== '' ? e.target.value : null }))}
                    value={filters.author}
                    allowClear
                />
            </div>
            <div className="filter-item">
                <Input.Search
                    placeholder="Search by genre"
                    onChange={(e) => setFilters((prev) => ({ ...prev, genre: e.target.value !== '' ? e.target.value : null }))}
                    value={filters.genre}
                    allowClear
                />
            </div>
            <div className="filter-item">
                <Select
                    options={Object.keys(bookStatusEnum).map((key) => ({
                        value: key,
                        label: bookStatusEnum[key]
                    }))}
                    onChange={(value) => setFilters((prev) => ({ ...prev, readingStatus: value }))}
                    placeholder="Select status"
                    value={filters.readingStatus}
                    allowClear
                />
            </div>
        </div>
    )
}